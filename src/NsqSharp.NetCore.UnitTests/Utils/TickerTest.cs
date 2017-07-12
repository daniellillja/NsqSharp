using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsqSharp.NetCore.UnitTestMethods;
using NsqSharp.Utils;
using NsqSharp.Utils.Channels;

namespace NsqSharp.Tests.Utils
{
    [TestClass]
    public class TickerTest
    {
        // NOTE: the default timer resolution on Windows is 15.6 ms
        private readonly TimeSpan AcceptableError = TimeSpan.FromMilliseconds(15.6);

        [TestMethod]
        public void TestSingleTicker()
        {
            // arrange
            var start = DateTime.Now;
            var ticker = new Ticker(TimeSpan.FromSeconds(1));

            // act
            bool ok;
            var sentAt = (DateTime)ticker.C.ReceiveOk(out ok);
            var duration = DateTime.Now - start;
            var offBy = DateTime.Now - sentAt;

            ticker.Stop();

            // assert
            Assert.IsTrue(ok, "ok");
            AssertHelper.GreaterOrEqual(duration, TimeSpan.FromSeconds(1) - AcceptableError, "duration");
            AssertHelper.Less(duration, TimeSpan.FromSeconds(1.5), "duration");
            AssertHelper.Less(offBy, TimeSpan.FromSeconds(0.5), "offBy");
        }

        [TestMethod]
        public void TestDoubleTicker()
        {
            // arrange
            var start = DateTime.Now;
            var ticker = new Ticker(TimeSpan.FromSeconds(1));

            // act
            bool ok1;
            var sentAt1 = (DateTime)ticker.C.ReceiveOk(out ok1);
            var duration1 = DateTime.Now - start;
            var offBy1 = DateTime.Now - sentAt1;

            bool ok2;
            var sentAt2 = (DateTime)ticker.C.ReceiveOk(out ok2);
            var duration2 = DateTime.Now - start;
            var offBy2 = DateTime.Now - sentAt2;

            ticker.Stop();

            // assert
            Assert.IsTrue(ok1, "ok1");
            AssertHelper.GreaterOrEqual(duration1, TimeSpan.FromSeconds(1) - AcceptableError, "duration1");
            AssertHelper.Less(duration1, TimeSpan.FromSeconds(1.5), "duration1");
            AssertHelper.Less(offBy1, TimeSpan.FromSeconds(0.5), "offBy1");

            Assert.IsTrue(ok2, "ok2");
            AssertHelper.GreaterOrEqual(duration2, TimeSpan.FromSeconds(2) - AcceptableError, "duration2");
            AssertHelper.Less(duration2, TimeSpan.FromSeconds(2.5), "duration2");
            AssertHelper.Less(offBy2, TimeSpan.FromSeconds(0.5), "offBy2");
        }

        [TestMethod]
        public void TestDoubleTickerWithStop()
        {
            // arrange
            var start = DateTime.Now;
            var ticker = new Ticker(TimeSpan.FromSeconds(1));

            // act
            bool ok1;
            var sentAt1 = (DateTime)ticker.C.ReceiveOk(out ok1);
            var duration1 = DateTime.Now - start;
            var offBy1 = DateTime.Now - sentAt1;

            ticker.Stop();

            var newTicker = new Ticker(TimeSpan.FromSeconds(5));
            bool? ok2 = null;
            Select
                .CaseReceiveOk(ticker.C, (d, b) => ok2 = false)
                .CaseReceive(newTicker.C, _ => ok2 = true)
                .NoDefault();

            newTicker.Stop();

            // assert
            Assert.IsTrue(ok1, "ok1");
            AssertHelper.GreaterOrEqual(duration1, TimeSpan.FromSeconds(1) - AcceptableError, "duration1");
            AssertHelper.Less(duration1, TimeSpan.FromSeconds(1.5), "duration1");
            AssertHelper.Less(offBy1, TimeSpan.FromSeconds(0.5), "offBy1");

            Assert.IsNotNull(ok2, "ok2");
            Assert.IsTrue(ok2.Value, "ok2");
        }

        [TestMethod]
        public void TestTickerLoopWithExitChan()
        {
            var start = DateTime.Now;
            var ticker = new Ticker(TimeSpan.FromSeconds(1));

            var listOfTimes = new List<TimeSpan>();
            var exitChan = new Chan<bool>();
            var lookupdRecheckChan = new Chan<bool>();
            var doLoop = true;
            using (var select =
                Select
                    .CaseReceive(ticker.C, o => listOfTimes.Add(DateTime.Now - start))
                    .CaseReceive(lookupdRecheckChan, o => listOfTimes.Add(DateTime.Now - start))
                    .CaseReceive(exitChan, o => doLoop = false)
                    .NoDefault(true))
            {
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                while (doLoop)
                {
                    select.Execute();
                    if (listOfTimes.Count >= 10)
                        GoFunc.Run(() => exitChan.Send(true), "exit notifier");
                }
            }

            ticker.Stop();

            var duration = DateTime.Now - start;

            Console.WriteLine("Duration: {0}", duration);
            foreach (var time in listOfTimes)
                Console.WriteLine("Tick: {0}", time);

            Assert.AreEqual(10, listOfTimes.Count, "listOfTimes.Count");
            AssertHelper.GreaterOrEqual(duration, TimeSpan.FromSeconds(10) - AcceptableError, "duration");
            AssertHelper.Less(duration, TimeSpan.FromSeconds(11));
        }

        [TestMethod]
        public void TestTickerLoopWithNemesisChan()
        {
            var start = DateTime.Now;
            var ticker = new Ticker(TimeSpan.FromSeconds(1));

            var listOfTimes = new List<TimeSpan>();
            var exitChan = new Chan<bool>();
            var lookupdRecheckChan = new Chan<bool>();
            var doLoop = true;
            using (var select =
                Select
                    .CaseReceive(ticker.C,
                        o =>
                        {
                            Console.WriteLine("Received tick");
                            listOfTimes.Add(DateTime.Now - start);

                            if (listOfTimes.Count == 5)
                                GoFunc.Run(() => lookupdRecheckChan.Send(true), "lookupd recheck sender");
                        })
                    .CaseReceive(lookupdRecheckChan,
                        o =>
                        {
                            Console.WriteLine("Nemesis");
                            Thread.Sleep(5000);
                        })
                    .CaseReceive(exitChan, o => doLoop = false)
                    .NoDefault(true))
            {
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                while (doLoop)
                {
                    select.Execute();
                    if (listOfTimes.Count >= 10)
                        GoFunc.Run(() => exitChan.Send(true), "exit notifier");
                }
            }

            ticker.Stop();

            var duration = DateTime.Now - start;

            Console.WriteLine("Duration: {0}", duration);
            foreach (var time in listOfTimes)
                Console.WriteLine("Tick: {0}", time);

            Assert.AreEqual(10, listOfTimes.Count, "listOfTimes.Count");
            AssertHelper.GreaterOrEqual(duration, TimeSpan.FromSeconds(14) - AcceptableError, "duration");
            AssertHelper.Less(duration, TimeSpan.FromSeconds(17));
        }

        [TestMethod]
        public void TestTickerLoopWithNemesisBufferedChan()
        {
            var start = DateTime.Now;
            var ticker = new Ticker(TimeSpan.FromSeconds(1));

            var x = 0;
            var listOfTimes = new List<TimeSpan>();
            var exitChan = new Chan<bool>();
            var lookupdRecheckChan = new Chan<bool>(1);
            var doLoop = true;
            using (var select =
                Select
                    .CaseReceive(ticker.C,
                        o =>
                        {
                            Console.WriteLine("Received tick");
                            listOfTimes.Add(DateTime.Now - start);

                            if (listOfTimes.Count == 5)
                                lookupdRecheckChan.Send(true);
                        })
                    .CaseReceive(lookupdRecheckChan,
                        o =>
                        {
                            Console.WriteLine("Nemesis");
                            for (var i = 0; i < 5; i++)
                            {
                                Thread.Sleep(1000);
                                Console.Write(".");
                            }
                            Console.WriteLine();
                        })
                    .CaseReceive(exitChan, o => doLoop = false)
                    .NoDefault(true))
            {
                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                while (doLoop)
                {
                    Console.WriteLine("start: {0} listOfTimes.Count: {1}", x, listOfTimes.Count);
                    select.Execute();
                    Console.WriteLine("finish: {0} listOfTimes.Count: {1}", x, listOfTimes.Count);
                    x++;
                    if (listOfTimes.Count >= 10)
                        GoFunc.Run(() => exitChan.Send(true), "exit notifier");
                }
            }

            ticker.Stop();

            var duration = DateTime.Now - start;

            Console.WriteLine("Duration: {0}", duration);
            foreach (var time in listOfTimes)
                Console.WriteLine("Tick: {0}", time);

            Assert.AreEqual(10, listOfTimes.Count, "listOfTimes.Count");
            Assert.IsTrue(duration >= TimeSpan.FromSeconds(14) - AcceptableError, "duration");
            Assert.IsTrue(duration < TimeSpan.FromSeconds(17));
        }

        [TestMethod]
        public void TestTickerStopRaceCondition()
        {
            // NOTE: This race condition was difficult to reproduce in Release but occurs
            //       almost immediately in Debug.

            var wg = new WaitGroup();
            var rand = new Random();

            const int tries = 1000;
            wg.Add(tries);
            for (var i = 0; i < tries; i++)
            {
                var time = rand.Next(1, 500);
                var ticker = new Ticker(TimeSpan.FromMilliseconds(time));
                Time.AfterFunc(TimeSpan.FromMilliseconds(time),
                    () =>
                    {
                        ticker.Close();
                        wg.Done();
                    });
            }
            wg.Wait();
        }
    }
}