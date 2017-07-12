using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsqSharp.Utils;
using NsqSharp.Utils.Channels;

namespace NsqSharp.Tests.Utils
{
    [TestClass]
    public class TimerTest
    {
        [TestMethod]
        public void TestTimerElapsed()
        {
            var timer = new Timer(TimeSpan.FromSeconds(1));
            var ticked = false;
            new SelectCase()
                .CaseReceive(timer.C, _ => ticked = true)
                .CaseReceive(Time.After(TimeSpan.FromSeconds(3)))
                .NoDefault();
            Assert.IsTrue(ticked);
        }

        [TestMethod]
        public void TestTimerDidNotElapse()
        {
            var timer = new Timer(TimeSpan.FromSeconds(3));
            var ticked = false;
            new SelectCase()
                .CaseReceive(timer.C, _ => ticked = true)
                .CaseReceive(Time.After(TimeSpan.FromSeconds(1)))
                .NoDefault();
            Assert.IsFalse(ticked);
        }

        [TestMethod]
        public void TestTimerStopRaceCondition()
        {
            // NOTE: This race condition was difficult to reproduce in Release but occurs
            //       almost immediately in Debug.

            var wg = new WaitGroup();
            var rand = new Random();

            var passed = true;

            const int tries = 1000;
            wg.Add(tries);
            for (var i = 0; i < tries; i++)
                GoFunc.Run(() =>
                {
                    try
                    {
                        var time = rand.Next(1, 2500);
                        var timer = new Timer(TimeSpan.FromMilliseconds(time));
                        Time.AfterFunc(TimeSpan.FromMilliseconds(time), () => timer.Stop());
                        timer.C.Receive();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        passed = false;
                    }
                    wg.Done();
                }, string.Format("timer {0}", i));
            wg.Wait();

            Assert.IsTrue(passed);
        }
    }
}