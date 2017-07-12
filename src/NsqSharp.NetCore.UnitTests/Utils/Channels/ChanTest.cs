﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsqSharp.NetCore.UnitTestMethods;
using NsqSharp.Utils;
using NsqSharp.Utils.Channels;

namespace NsqSharp.Tests.Utils.Channels
{
    [TestClass]
    public class ChanTest
    {
        [TestMethod]
        public void SingleNumberGenerator()
        {
            var c = new Chan<int>();

            var t = new Thread(() =>
            {
                for (var i = 0; i < 10; i++)
                    c.Send(i);
                c.Close();
            });
            t.IsBackground = true;
            t.Start();

            var list = new List<int>();
            foreach (var i in c)
                list.Add(i);

            AssertHelper.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, list);
        }

        [TestMethod]
        public void MultipleNumberGenerators()
        {
            var c = new Chan<int>();

            for (var i = 0; i < 10; i++)
            {
                var localNum = i;

                var t = new Thread(() => c.Send(localNum));
                t.IsBackground = true;
                t.Start();
            }

            var list = new List<int>();
            for (var i = 0; i < 10; i++)
                list.Add(c.Receive());

            list.Sort();

            AssertHelper.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, list);
        }

        [TestMethod]
        public void PrimeSieve()
        {
            var generate = new Action<Chan<int>>(cgen =>
            {
                while (true)
                    for (var i = 2; ; i++)
                        cgen.Send(i);
                // ReSharper disable once FunctionNeverReturns
            });

            var filter = new Action<Chan<int>, Chan<int>, int>((cin, cout, prime) =>
            {
                while (true)
                {
                    var i = cin.Receive();
                    if (i % prime != 0)
                        cout.Send(i);
                }
                // ReSharper disable once FunctionNeverReturns
            });

            var ch = new Chan<int>();

            var generateCh = ch;
            var threadGenerate = new Thread(() => generate(generateCh));
            threadGenerate.IsBackground = true;
            threadGenerate.Start();

            var list = new List<int>();

            for (var i = 0; i < 10; i++)
            {
                var prime = ch.Receive();
                list.Add(prime);

                var ch0 = ch;
                var ch1 = new Chan<int>();

                var threadFilter = new Thread(() => filter(ch0, ch1, prime));
                threadFilter.IsBackground = true;
                threadFilter.Start();

                ch = ch1;
            }

            AssertHelper.AreEqual(new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 }, list);
        }

        [TestMethod]
        public void SelectTwoChannels()
        {
            var c1 = new Chan<int>();
            var c2 = new Chan<int>();

            var t1 = new Thread(() =>
            {
                Thread.Sleep(10);
                c1.Send(1);
            });
            t1.IsBackground = true;

            var t2 = new Thread(() => c2.Send(2));
            t2.IsBackground = true;

            var list = new List<int>();

            t2.Start();
            t1.Start();

            Select
                .CaseReceive(c1, list.Add)
                .CaseReceive(c2, list.Add)
                .NoDefault();

            Assert.AreEqual(1, list.Count, "list.Count");
            Assert.AreEqual(2, list[0], "list[0]");
        }

        [TestMethod]
        public void SelectNullChannel()
        {
            var c1 = new Chan<int>();

            var t1 = new Thread(() =>
            {
                Thread.Sleep(10);
                c1.Send(3);
            });
            t1.IsBackground = true;

            var list = new List<int>();

            t1.Start();

            Select
                .CaseReceive((Chan<int>)null, list.Add)
                .CaseReceive(c1, list.Add)
                .NoDefault();

            Assert.AreEqual(1, list.Count, "list.Count");
            Assert.AreEqual(3, list[0], "list[0]");
        }

        [TestMethod]
        public void SelectDefaultCaseNoChannelsReady()
        {
            var c1 = new Chan<int>();
            var c2 = new Chan<int>();

            var t1 = new Thread(() =>
            {
                Thread.Sleep(10);
                c1.Send(1);
            });
            t1.IsBackground = true;

            var t2 = new Thread(() =>
            {
                Thread.Sleep(10);
                c2.Send(2);
            });
            t2.IsBackground = true;

            var list = new List<int>();

            t1.Start();
            t2.Start();

            Select
                .CaseReceive(c1, list.Add)
                .CaseReceive(c2, list.Add)
                .Default(() => list.Add(3));

            Assert.AreEqual(1, list.Count, "list.Count");
            Assert.AreEqual(3, list[0], "list[0]");
        }

        [TestMethod]
        public void SelectDefaultCaseChannelReady()
        {
            var c1 = new Chan<int>();
            var c2 = new Chan<int>();

            var t1 = new Thread(() =>
            {
                Thread.Sleep(100);
                c1.Send(1);
            });
            t1.IsBackground = true;

            var t2 = new Thread(() => c2.Send(5));
            t2.IsBackground = true;

            var list = new List<int>();

            t1.Start();
            t2.Start();

            Thread.Sleep(50);

            Select
                .CaseReceive(c1, list.Add)
                .CaseReceive(c2, list.Add)
                .Default(() => list.Add(3));

            Assert.AreEqual(1, list.Count, "list.Count");
            Assert.AreEqual(5, list[0], "list[0]");
        }

        [TestMethod]
        [ExpectedException(typeof(ChannelClosedException))]
        public void SendOnClosedChannelThrows()
        {
            var c = new Chan<int>();
            c.Close();

            c.Send(1);
        }

        [TestMethod]
        public void ReceiveOnClosedChannelReturnsDefault()
        {
            var c = new Chan<int>();
            c.Close();

            var val = c.Receive();
            Assert.AreEqual(default(int), val);
        }

        [TestMethod]
        public void SelectSendsOnly()
        {
            var c1 = new Chan<int>();
            var c2 = new Chan<int>();

            var t1 = new Thread(() =>
            {
                Thread.Sleep(1000);
                c1.Receive();
            });
            t1.IsBackground = true;

            var t2 = new Thread(() => c2.Receive());
            t2.IsBackground = true;

            var list = new List<int>();

            t1.Start();
            t2.Start();

            Select
                .CaseSend(c1, 1, () => list.Add(1))
                .CaseSend(c2, 2, () => list.Add(2))
                .NoDefault();

            Assert.AreEqual(1, list.Count, "list.Count");
            Assert.AreEqual(2, list[0], "list[0]");
        }

        [TestMethod]
        public void SingleNumberGeneratorIEnumerableChan()
        {
            var c = new Chan<int>();

            var t = new Thread(() =>
            {
                for (var i = 0; i < 10; i++)
                    c.Send(i);
                c.Close();
            });
            t.IsBackground = true;
            t.Start();

            var list = new List<int>();
            foreach (var i in (IEnumerable)c)
                list.Add((int)i);

            AssertHelper.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, list);
        }

        [TestMethod]
        public void SelectSendAndReceiveReceiveReady()
        {
            var listLocker = new object();

            var c1 = new Chan<int>();
            var c2 = new Chan<int>();

            var t1 = new Thread(() =>
            {
                Thread.Sleep(150);
                c1.Send(1);
            });
            t1.IsBackground = true;

            var list = new List<int>();

            var t2 = new Thread(() =>
            {
                var val = c2.Receive();
                lock (listLocker)
                {
                    list.Add(val);
                }
            });
            t2.IsBackground = true;

            t1.Start();
            t2.Start();

            Select
                .CaseReceive(c1, o =>
                {
                    lock (listLocker)
                    {
                        list.Add(o);
                    }
                })
                .CaseSend(c2, 2)
                .NoDefault();

            Thread.Sleep(20);

            lock (listLocker)
            {
                Assert.AreEqual(1, list.Count, "list.Count");
                Assert.AreEqual(2, list[0], "list[0]");
            }
        }

        [TestMethod]
        public void SelectSendAndReceiveSendReady()
        {
            var c1 = new Chan<int>();
            var c2 = new Chan<int>();

            var t1 = new Thread(() => c1.Send(1));
            t1.IsBackground = true;

            var list = new List<int>();

            var t2 = new Thread(() =>
            {
                Thread.Sleep(50);
                list.Add(c2.Receive());
            });
            t2.IsBackground = true;

            t1.Start();
            t2.Start();

            Select
                .CaseReceive(c1, list.Add)
                .CaseSend(c2, 2, () => { })
                .NoDefault();

            Assert.AreEqual(1, list.Count, "list.Count");
            Assert.AreEqual(1, list[0], "list[0]");
        }

        [TestMethod]
        public void TwoSelectsSendAndReceiveCanTalk()
        {
            var c = new Chan<int>();

            var actual = 0;

            var wg = new WaitGroup();
            wg.Add(2);

            GoFunc.Run(() =>
            {
                Select
                    .CaseSend(c, 7)
                    .NoDefault();

                wg.Done();
            }, "sender");

            GoFunc.Run(() =>
            {
                Select
                    .CaseReceive(c, o => actual = o)
                    .NoDefault();

                wg.Done();
            }, "receiver");

            wg.Wait();

            Assert.AreEqual(7, actual);
        }

        [TestMethod]
        public void BufferedChannelsDontBlock()
        {
            var c = new Chan<int>(1);
            c.Send(2);
            var actual = c.Receive();

            Assert.AreEqual(2, actual);
        }

        [TestMethod]
        public void BufferedChannelsSelectSendAndReceiveInGoroutine()
        {
            var c = new Chan<int>(10);

            var list = new List<int>();
            var wg = new WaitGroup();

            wg.Add(2);

            GoFunc.Run(() =>
            {
                var doLoop = true;
                while (doLoop)
                    Select
                        .CaseReceiveOk(c, (i, ok) =>
                        {
                            if (ok)
                                list.Add(i);
                            else
                                doLoop = false;
                        })
                        .NoDefault();

                wg.Done();
            }, "bufferChannelsTest:receiveLoop");

            GoFunc.Run(() =>
            {
                for (var i = 0; i < 10; i++)
                    Select
                        .CaseSend(c, i)
                        .NoDefault();

                c.Close();
                wg.Done();
            }, "bufferedChannelsTest:sendLoop");

            wg.Wait();

            AssertHelper.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, list);
        }

        [TestMethod]
        public void BufferedChannelsSelectSendInGoroutine()
        {
            var c = new Chan<int>(10);

            var list = new List<int>();
            var wg = new WaitGroup();

            wg.Add(1);

            GoFunc.Run(() =>
            {
                for (var i = 0; i < 10; i++)
                    Select
                        .CaseSend(c, i)
                        .NoDefault();

                c.Close();
                wg.Done();
            }, "bufferedChannelsTest:sendLoop");

            wg.Wait();

            var doLoop = true;
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (doLoop)
                Select
                    .CaseReceiveOk(c, (i, ok) =>
                    {
                        if (ok)
                            list.Add(i);
                        else
                            doLoop = false;
                    })
                    .NoDefault();

            AssertHelper.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, list);
        }

        [TestMethod]
        public void BufferedChannelsSelectReceiveInGoroutine()
        {
            var c = new Chan<int>(10);

            var list = new List<int>();
            var wg = new WaitGroup();

            wg.Add(1);

            GoFunc.Run(() =>
            {
                var doLoop = true;
                while (doLoop)
                    Select
                        .CaseReceiveOk(c, (i, ok) =>
                        {
                            if (ok)
                                list.Add(i);
                            else
                                doLoop = false;
                        })
                        .NoDefault();

                wg.Done();
            }, "bufferChannelsTest:receiveLoop");

            for (var i = 0; i < 10; i++)
                Select
                    .CaseSend(c, i)
                    .NoDefault();

            c.Close();

            wg.Wait();

            AssertHelper.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, list);
        }

        [TestMethod]
        public void BufferedChannelsReceiveSelectInGoroutineSendOnMainThread()
        {
            var c = new Chan<int>(10);

            var list = new List<int>();
            var wg = new WaitGroup();

            wg.Add(1);

            GoFunc.Run(() =>
            {
                var doLoop = true;
                while (doLoop)
                    Select
                        .CaseReceiveOk(c, (i, ok) =>
                        {
                            if (ok)
                                list.Add(i);
                            else
                                doLoop = false;
                        })
                        .NoDefault();

                wg.Done();
            }, "bufferChannelsTest:receiveLoop");

            for (var i = 0; i < 10; i++)
                c.Send(i);

            c.Close();

            wg.Wait();

            AssertHelper.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, list);
        }

        [TestMethod]
        public void ClosedChannelsWithDataShouldNotReportClosedUntilDrained()
        {
            for (var i = 0; i < 10000; i++)
            {
                var chan = new Chan<int>();

                var wait = new AutoResetEvent(false);

                GoFunc.Run(() =>
                {
                    chan.Send(1);
                    chan.Close();
                }, "send");

                bool ok = false, ok2 = false;
                int actual = -1, actual2 = -1;

                GoFunc.Run(() =>
                {
                    actual = chan.ReceiveOk(out ok);
                    actual2 = chan.ReceiveOk(out ok2);
                    wait.Set();
                }, "receive");

                wait.WaitOne();

                Assert.AreEqual(true, ok, string.Format("ok iteration {0}", i));
                Assert.AreEqual(1, actual, string.Format("actual iteration {0}", i));
                Assert.AreEqual(false, ok2, string.Format("ok2 iteration {0}", i));
                Assert.AreEqual(default(int), actual2, string.Format("actual2 iteration {0}", i));
            }
        }

        [TestMethod]
        public void MultiThreadedSelectTestWithDefer()
        {
            var c1 = new Chan<int>();
            var c2 = new Chan<int>();
            var done = new Chan<bool>();
            var start = new Chan<bool>();

            var c1Count = 0;
            var c2Count = 0;

            var count = 0;
            var totalReceived = 0;

            Action receive = () =>
            {
                start.Receive();

                var val1 = 0;
                var val2 = 0;
                var doLoop = true;

                var select =
                    Select
                        .CaseReceive(c1, i => val1 = i)
                        .CaseReceive(c2, i => val2 = i)
                        .CaseReceive(done, b => doLoop = false)
                        .NoDefault(true);

                while (doLoop)
                {
                    val1 = 0;
                    val2 = 0;

                    select.Execute();

                    if (doLoop)
                    {
                        Assert.IsTrue(val1 == 0 || val2 == 0, "val1 == 0 || val2 == 0");
                        Assert.IsTrue(val1 == 1 || val2 == 2, "val1 == 1 || val2 == 2");
                    }

                    Interlocked.Increment(ref totalReceived);
                }
            };

            Action send = () =>
            {
                start.Receive();

                var select =
                    Select
                        .CaseSend(c1, 1, () => Interlocked.Increment(ref c1Count))
                        .CaseSend(c2, 2, () => Interlocked.Increment(ref c2Count))
                        .NoDefault(true);

                while (count < 30000)
                {
                    Interlocked.Increment(ref count);
                    select.Execute();
                }

                done.Close();
            };

            for (var i = 0; i < 8; i++)
            {
                GoFunc.Run(receive, "receiver");
                GoFunc.Run(send, "sender");
            }

            start.Close();
            done.Receive();

            Assert.IsTrue(count >= 30000);
            Assert.IsTrue(totalReceived > 29900);
        }

        [TestMethod]
        public void MultiThreadedSelectTestWithoutDefer()
        {
            var c1 = new Chan<int>();
            var c2 = new Chan<int>();
            var done = new Chan<bool>();
            var start = new Chan<bool>();

            var c1Count = 0;
            var c2Count = 0;

            var count = 0;
            var totalReceived = 0;

            Action receive = () =>
            {
                start.Receive();

                var val1 = 0;
                var val2 = 0;
                var doLoop = true;

                while (doLoop)
                {
                    val1 = 0;
                    val2 = 0;

                    Select
                        .CaseReceive(c1, i => val1 = i)
                        .CaseReceive(c2, i => val2 = i)
                        .CaseReceive(done, b => doLoop = false)
                        .NoDefault();

                    if (doLoop)
                    {
                        Assert.IsTrue(val1 == 0 || val2 == 0, "val1 == 0 || val2 == 0");
                        Assert.IsTrue(val1 == 1 || val2 == 2, "val1 == 1 || val2 == 2");
                    }

                    Interlocked.Increment(ref totalReceived);
                }
            };

            Action send = () =>
            {
                start.Receive();

                while (count < 10000)
                {
                    Interlocked.Increment(ref count);

                    Select
                        .CaseSend(c1, 1, () => Interlocked.Increment(ref c1Count))
                        .CaseSend(c2, 2, () => Interlocked.Increment(ref c2Count))
                        .NoDefault();

                    Select
                        .CaseSend(c2, 2, () => Interlocked.Increment(ref c2Count))
                        .CaseSend(c1, 1, () => Interlocked.Increment(ref c1Count))
                        .NoDefault();
                }

                done.Close();
            };

            for (var i = 0; i < 8; i++)
            {
                GoFunc.Run(receive, "receiver");
                GoFunc.Run(send, "sender");
            }

            start.Close();
            done.Receive();

            Assert.IsTrue(count >= 10000);
            Assert.IsTrue(totalReceived > 19900);
        }
    }
}