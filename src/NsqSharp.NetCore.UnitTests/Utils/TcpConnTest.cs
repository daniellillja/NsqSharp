using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsqSharp.NetCore.UnitTestMethods;
using NsqSharp.Utils;

namespace NsqSharp.Tests.Utils
{
    [TestClass]
    public class TcpConnTest
    {
        [TestMethod]
        public void TestTcpConnHappyPath()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 4192);
            tcpListener.Start();

            var wg = new WaitGroup();
            wg.Add(1);

            GoFunc.Run(() =>
            {
                var tcpClient = tcpListener.AcceptTcpClientAsync().Result;

                using (var rdr = new BinaryReader(tcpClient.GetStream()))
                using (var connw = new BinaryWriter(tcpClient.GetStream()))
                {
                    while (true)
                    {
                        var readMsg = ReadBytes(rdr, (byte)'\n');
                        if (readMsg.SequenceEqual(Encoding.UTF8.GetBytes("QUIT\n")))
                            break;
                        connw.Write(readMsg);
                    }
                }

                tcpClient.Dispose();
                wg.Done();
            }, "TcpConnTest read loop");

            var tcpConn = new TcpConn(IPAddress.Loopback.ToString(), 4192);

            var helloMsg = Encoding.UTF8.GetBytes("Hello\n");
            tcpConn.Write(helloMsg, 0, helloMsg.Length);

            var recv = new byte[helloMsg.Length];
            tcpConn.Read(recv);
            Console.WriteLine(Encoding.UTF8.GetString(recv));

            var quitMsg = Encoding.UTF8.GetBytes("QUIT\n");
            tcpConn.Write(quitMsg, 0, quitMsg.Length);

            recv = new byte[quitMsg.Length];
            tcpConn.Read(recv);
            Console.WriteLine(Encoding.UTF8.GetString(recv));

            wg.Wait();

            tcpConn.Close();
        }

        [TestMethod]
        public void TestTcpConnWriteAfterClose()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 4193);
            tcpListener.Start();

            var wg = new WaitGroup();
            wg.Add(1);

            GoFunc.Run(() =>
            {
                var tcpClient = tcpListener.AcceptTcpClientAsync().Result;

                using (var rdr = new BinaryReader(tcpClient.GetStream()))
                using (var connw = new BinaryWriter(tcpClient.GetStream()))
                {
                    while (true)
                    {
                        var readMsg = ReadBytes(rdr, (byte)'\n');
                        if (readMsg.SequenceEqual(Encoding.UTF8.GetBytes("QUIT\n")))
                            break;
                        connw.Write(readMsg);
                    }
                }

                tcpClient.Dispose();
                wg.Done();
            }, "TcpConnTest read loop");

            var tcpConn = new TcpConn(IPAddress.Loopback.ToString(), 4193);

            var helloMsg = Encoding.UTF8.GetBytes("Hello\n");
            tcpConn.Write(helloMsg, 0, helloMsg.Length);

            var recv = new byte[helloMsg.Length];
            tcpConn.Read(recv);
            Console.WriteLine(Encoding.UTF8.GetString(recv));

            var quitMsg = Encoding.UTF8.GetBytes("QUIT\n");
            tcpConn.Write(quitMsg, 0, quitMsg.Length);

            recv = new byte[quitMsg.Length];
            tcpConn.Read(recv);
            Console.WriteLine(Encoding.UTF8.GetString(recv));

            wg.Wait();

            tcpConn.Close();

            AssertHelper.Throws<ConnectionClosedException>(() => tcpConn.Write(quitMsg, 0, quitMsg.Length));
        }

        [TestMethod]
        public void TestTcpConnReadAfterClose()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 4194);
            tcpListener.Start();

            var wg = new WaitGroup();
            wg.Add(1);

            GoFunc.Run(() =>
            {
                var tcpClient = tcpListener.AcceptTcpClientAsync().Result;

                using (var rdr = new BinaryReader(tcpClient.GetStream()))
                using (var connw = new BinaryWriter(tcpClient.GetStream()))
                {
                    while (true)
                    {
                        var readMsg = ReadBytes(rdr, (byte)'\n');
                        if (readMsg.SequenceEqual(Encoding.UTF8.GetBytes("QUIT\n")))
                            break;
                        connw.Write(readMsg);
                    }
                }

                tcpClient.Dispose();
                wg.Done();
            }, "TcpConnTest read loop");

            var tcpConn = new TcpConn(IPAddress.Loopback.ToString(), 4194);

            var helloMsg = Encoding.UTF8.GetBytes("Hello\n");
            tcpConn.Write(helloMsg, 0, helloMsg.Length);

            var recv = new byte[helloMsg.Length];
            tcpConn.Read(recv);
            Console.WriteLine(Encoding.UTF8.GetString(recv));

            var quitMsg = Encoding.UTF8.GetBytes("QUIT\n");
            tcpConn.Write(quitMsg, 0, quitMsg.Length);

            recv = new byte[quitMsg.Length];
            tcpConn.Close();

            AssertHelper.Throws<ConnectionClosedException>(() => tcpConn.Read(recv));

            wg.Wait();
        }

        [TestMethod]
        public void TestTcpConnFlushAfterClose()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 4195);
            tcpListener.Start();

            var wg = new WaitGroup();
            wg.Add(1);

            GoFunc.Run(() =>
            {
                var tcpClient = tcpListener.AcceptTcpClientAsync().Result;

                using (var rdr = new BinaryReader(tcpClient.GetStream()))
                using (var connw = new BinaryWriter(tcpClient.GetStream()))
                {
                    while (true)
                    {
                        var readMsg = ReadBytes(rdr, (byte)'\n');
                        if (readMsg.SequenceEqual(Encoding.UTF8.GetBytes("QUIT\n")))
                            break;
                        connw.Write(readMsg);
                    }
                }

                tcpClient.Dispose();
                wg.Done();
            }, "TcpConnTest read loop");

            var tcpConn = new TcpConn(IPAddress.Loopback.ToString(), 4195);

            var helloMsg = Encoding.UTF8.GetBytes("Hello\n");
            tcpConn.Write(helloMsg, 0, helloMsg.Length);

            var recv = new byte[helloMsg.Length];
            tcpConn.Read(recv);
            Console.WriteLine(Encoding.UTF8.GetString(recv));

            var quitMsg = Encoding.UTF8.GetBytes("QUIT\n");
            tcpConn.Write(quitMsg, 0, quitMsg.Length);

            tcpConn.Close();

            AssertHelper.Throws<ConnectionClosedException>(() => tcpConn.Flush());

            wg.Wait();
        }

        private byte[] ReadBytes(BinaryReader rdr, byte stop)
        {
            using (var memoryStream = new MemoryStream())
            {
                while (true)
                {
                    var b = rdr.ReadByte();
                    memoryStream.WriteByte(b);
                    if (b == stop)
                        return memoryStream.ToArray();
                }
            }
        }
    }
}