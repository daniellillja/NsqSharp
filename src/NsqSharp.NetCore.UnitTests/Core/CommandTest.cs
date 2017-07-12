﻿using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsqSharp.Core;
using NsqSharp.Utils;

namespace NsqSharp.Tests.Core
{
    [TestClass]
    public class CommandTest
    {
        [TestMethod]
        public void Benchmark()
        {
            const int benchmarkNum = 200000; // MemoryStream starts to choke after ~1GB
            var data = new byte[2048];
            var cmd = Command.Publish("test", data);

            var writer = new MemoryStreamWriter();

            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < benchmarkNum; i++)
                cmd.WriteTo(writer);
            stopwatch.Stop();

            Console.WriteLine(string.Format("{0:#,0} commands written in {1:mm\\:ss\\.fff}; Avg: {2:#,0}/s",
                benchmarkNum, stopwatch.Elapsed, benchmarkNum / stopwatch.Elapsed.TotalSeconds));
        }

        private class MemoryStreamWriter : IWriter
        {
            private readonly MemoryStream _memoryStream = new MemoryStream();

            public int Write(byte[] b, int offset, int length)
            {
                _memoryStream.Write(b, offset, length);
                return length;
            }
        }
    }
}