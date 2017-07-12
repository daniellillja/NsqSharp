﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsqSharp.Api;
using NsqSharp.Core;
using NsqSharp.NetCore.UnitTestMethods;
using NsqSharp.TestMethods.TestMethodHelpers;
using NsqSharp.Utils;
using NsqSharp.Utils.Extensions;

namespace NsqSharp.Tests
{
    [TestClass]
    public class ConsumerTest
    {
        private static readonly NsqdHttpClient _nsqdHttpClient;
        private static readonly NsqLookupdHttpClient _nsqLookupdHttpClient;

        static ConsumerTest()
        {
            _nsqdHttpClient = new NsqdHttpClient("127.0.0.1:4151", TimeSpan.FromSeconds(5));
            _nsqLookupdHttpClient = new NsqLookupdHttpClient("127.0.0.1:4161", TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        public void TestConsumer()
        {
            consumerTest(configSetter: null);
        }

        [TestMethod]
        public void TestConsumerTLS()
        {
            consumerTest(c =>
                         {
                             c.TlsConfig = new TlsConfig { InsecureSkipVerify = true };
                         });
        }

        [TestMethod]
        public void TestConsumerTLSViaSet()
        {
            consumerTest(c =>
                         {
                             c.Set("tls_insecure_skip_verify", true);
                         });
        }


        // TODO: Deflate
        /*[Test, Ignore("Deflate not implemented")]
        public void TestConsumerDeflate()
        {
            consumerTest(c =>
                         {
                             c.Deflate = true;
                         });
        }*/

        // TODO: Snappy
        /*[Test, Ignore("Snappy not implemented")]
        public void TestConsumerSnappy()
        {
            consumerTest(c =>
                         {
                             c.Snappy = true;
                         });
        }*/

        // TODO: TLS/Deflate
        /*[Test, Ignore("TLS/Deflate not implemented")]
        public void TestConsumerTLSDeflate()
        {
            consumerTest(c =>
            {
                c.TlsV1 = true;
                c.TlsConfig = new TlsConfig { InsecureSkipVerify = true };
                c.Deflate = true;
            });
        }*/

        // TODO: TLS/Snappy
        /*[Test, Ignore("TLS/Snappy not implemented")]
        public void TestConsumerTLSSnappy()
        {
            consumerTest(c =>
            {
                c.TlsV1 = true;
                c.TlsConfig = new TlsConfig { InsecureSkipVerify = true };
                c.Snappy = true;
            });
        }*/

        /*
        // TODO: Address when client certificate auth enabled
        [TestMethod]
        public void TestConsumerTLSClientCert()
        {
            consumerTest(c =>
            {
                c.TlsConfig = new TlsConfig
                {
                    //Certificates = cert
                    InsecureSkipVerify = true
                };
            });
        }

        // TODO: Address when client certificate auth enabled
        [TestMethod]
        public void TestConsumerTLSClientCertViaSet()
        {
            consumerTest(c =>
            {                
                c.Set("ts_v1", true);
                c.Set("tls_cert", "./test/client.pem");
                c.Set("tls_key", "./test/client.key");
                c.Set("tls_insecure_skip_verify", true);
            });
        }*/

        private void consumerTest(Action<NsqConfig> configSetter)
        {
            var config = new NsqConfig();
            // so that the test can simulate reaching max requeues and a call to LogFailedMessage
            config.DefaultRequeueDelay = TimeSpan.Zero;
            // so that the test wont timeout from backing off
            config.MaxBackoffDuration = TimeSpan.FromMilliseconds(50);
            if (configSetter != null)
            {
                configSetter(config);
            }
            var topicName = "rdr_test";

            // TODO: Deflate, Snappy
            /*if (config.Deflate)
                topicName = topicName + "_deflate";
            else if (config.Snappy)
                topicName = topicName + "_snappy";*/

            if (config.TlsConfig != null)
                topicName = topicName + "_tls";

            topicName = topicName + DateTime.Now.Unix();

            try
            {
                var q = new Consumer(topicName, "ch", new TestConsoleLogger(), config);

                var h = new MyTestHandler { q = q };
                q.AddHandler(h);

                SendMessage(topicName, "put", "{\"msg\":\"single\"}");
                SendMessage(topicName, "mput", "{\"msg\":\"double\"}\n{\"msg\":\"double\"}");
                SendMessage(topicName, "put", "TOBEFAILED");
                h.messagesSent = 4;

                const string addr = "127.0.0.1:4150";
                q.ConnectToNsqd(addr);

                var stats = q.GetStats();
                Assert.AreNotEqual(0, stats.Connections, "stats report 0 connections (should be > 0)");

                // NOTE: changed to just return without throwing; throwing Exceptions is a little more disruptive in .NET
                // than returning err in Go
                //AssertHelper.Throws<ErrAlreadyConnected>(() => q.ConnectToNSQD(addr),
                //    "should not be able to connect to the same NSQ twice");

                AssertHelper.Throws<ErrNotConnected>(() => q.DisconnectFromNsqd("1.2.3.4:4150"),
                    "should not be able to disconnect from an unknown nsqd");

                AssertHelper.Throws<TimeoutException>(() => q.ConnectToNsqd("1.2.3.4:4150"),
                    "should not be able to connect to non-existent nsqd");

                // should be able to disconnect from an nsqd
                q.DisconnectFromNsqd("1.2.3.4:4150");
                q.DisconnectFromNsqd("127.0.0.1:4150");

                q.Wait();

                stats = q.GetStats();

                Assert.AreEqual(h.messagesReceived + h.messagesFailed, stats.MessagesReceived, "stats report messages received");
                Assert.AreEqual(8, h.messagesReceived, "messages received");
                Assert.AreEqual(4, h.messagesSent, "messages sent");
                Assert.AreEqual(1, h.messagesFailed, "failed messaged not done");
            }
            finally
            {
                _nsqdHttpClient.DeleteTopic(topicName);
                _nsqLookupdHttpClient.DeleteTopic(topicName);
            }
        }

        private void SendMessage(string topic, string method, string msg)
        {
            byte[] body = Encoding.UTF8.GetBytes(msg);

            var w = new Producer("127.0.0.1:4150");
            if (method == "put")
            {
                w.Publish(topic, body);
            }
            else if (method == "mput")
            {
                // split by '\n'
                var mpub = new List<byte[]>();
                var ms = new MemoryStream();
                for (int i = 0; i < body.Length; i++)
                {
                    if (body[i] == '\n')
                    {
                        mpub.Add(ms.ToArray());
                        ms.Dispose();
                        ms = new MemoryStream();
                    }
                    else
                    {
                        ms.WriteByte(body[i]);
                    }
                }
                mpub.Add(ms.ToArray());
                ms.Dispose();

                w.MultiPublish(topic, mpub);
            }
            w.Stop();
        }

        public class MyTestHandler : IHandler
        {
            public Consumer q { get; set; }
            public int messagesSent { get; set; }
            public int messagesReceived { get; set; }
            public int messagesFailed { get; set; }

            public void LogFailedMessage(IMessage message)
            {
                messagesFailed++;
                q.StopAsync();
            }

            public void HandleMessage(IMessage message)
            {
                string body = Encoding.UTF8.GetString(message.Body);

                if (body == "TOBEFAILED")
                {
                    messagesReceived++;
                    throw new Exception("fail this message");
                }

                string msg;

                var serializer = new DataContractJsonSerializer(typeof(MessagePayload));
                using (var memoryStream = new MemoryStream(message.Body))
                {
                    var msgPayload = (MessagePayload)serializer.ReadObject(memoryStream);
                    msg = msgPayload.msg;
                }

                if (msg != "single" && msg != "double")
                {
                    throw new Exception(string.Format("message 'action' was not correct: {0} {1}", msg, body));
                }
                messagesReceived++;
            }
        }

        [DataContract]
        public class MessagePayload
        {
            [DataMember]
            public string msg { get; set; }
        }
    }
}
