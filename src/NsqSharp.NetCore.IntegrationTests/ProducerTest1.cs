using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NsqSharp.NetCore.IntegrationTests
{
    [TestClass]
    public class ProducerTest1
    {
        [TestMethod]
        public void ConsumerGetsMessageFromProducer()
        {
            var message = "Hello!";
            // Create a Producer. Producers must connect to nsqd directly (:4150)
            var producer = new Producer("127.0.0.1:4150");
            // Publish message "Hello!" to topic "test-topic-name"

            // Create a new Consumer listening to topic "test-topic-name" on channel "channel-name"
            var consumer = new Consumer("test-topic-name", "channel-name");

            var handler = new MessageHandler();
            // Add a handler to handle incoming messages
            consumer.AddHandler(handler);
            // Consumers can connect to nsqd (:4150) or nsqlookupd (:4161)
            // When connecting to nsqlookupd to default polling interval for topic producers is 60s. This
            // can be modified with the constructor overload which takes a Config parameter.
            consumer.ConnectToNsqLookupd("127.0.0.1:4161");

            producer.Publish("test-topic-name", message);

            Thread.Sleep(1000);
            handler.AssertMessageRecieved(message);

            // Stop Producer/Consumer
            producer.Stop();
            consumer.Stop();
        }

        public class MessageHandler : IHandler
        {
            private List<IMessage> _messagesRecieved = new List<IMessage>();

            /// <summary>
            /// Handles a message.
            /// </summary>
            /// <param name="message">The message.</param>
            public void HandleMessage(IMessage message)
            {
                _messagesRecieved.Add(message);
            }

            /// <summary>
            /// Called when a <see cref="Message"/> has exceeded the Consumer specified <see cref="NsqConfig.MaxAttempts"/>.
            /// </summary>
            /// <param name="message">The failed message.</param>
            public void LogFailedMessage(IMessage message)
            {
            }

            public void AssertMessageRecieved(string content)
            {
                Assert.IsTrue(_messagesRecieved.Any(m => Encoding.UTF8.GetString(m.Body).Equals(content)));
            }
        }
    }
}
