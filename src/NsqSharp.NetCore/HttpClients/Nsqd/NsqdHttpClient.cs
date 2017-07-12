using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using NsqSharp.Utils;

namespace NsqSharp.Api
{
    /// <summary>An nsqd HTTP client.</summary>
    public class NsqdHttpClient : NsqHttpApi
    {
        private readonly int _timeoutMilliseconds;

        /// <summary>Initializes a new instance of <see cref="NsqLookupdHttpClient" /> class.</summary>
        /// <param name="nsqdHttpAddress">The nsqd HTTP address, including port. Example: 127.0.0.1:4151</param>
        /// <param name="httpRequestTimeout">The HTTP request timeout.</param>
        public NsqdHttpClient(string nsqdHttpAddress, TimeSpan httpRequestTimeout)
            : base(nsqdHttpAddress, httpRequestTimeout)
        {
            _timeoutMilliseconds = (int)httpRequestTimeout.TotalMilliseconds;
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="message">The message.</param>
        /// <returns>The response from the nsqd HTTP server.</returns>
        public string Publish(string topic, string message)
        {
            ValidateTopic(topic);
            if (message == null)
                throw new ArgumentNullException("message");

            return Publish(topic, Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="message">The message.</param>
        /// <returns>The response from the nsqd HTTP server.</returns>
        public string Publish(string topic, byte[] message)
        {
            ValidateTopic(topic);
            if (message == null)
                throw new ArgumentNullException("message");

            string route = string.Format("/pub?topic={0}", topic);
            return Post(route, message);
        }

        /// <summary>
        /// Publishes multiple messages. More efficient than calling Publish several times for the same message type.
        /// See http://nsq.io/components/nsqd.html#mpub.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The response from the nsqd HTTP server.</returns>
        public string PublishMultiple(string topic, IEnumerable<string> messages)
        {
            ValidateTopic(topic);
            if (messages == null)
                throw new ArgumentNullException("messages");

            var messagesArray = messages.ToArray();
            if (messagesArray.Length == 0)
                return null;

            string body = string.Join("\n", messagesArray);

            string route = string.Format("/mpub?topic={0}", topic);
            return Post(route, Encoding.UTF8.GetBytes(body));
        }

        /// <summary>
        /// Publishes multiple messages. More efficient than calling Publish several times for the same message type.
        /// See http://nsq.io/components/nsqd.html#mpub.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="messages">The messages.</param>
        /// <returns>The response from the nsqd HTTP server.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public string PublishMultiple(string topic, IEnumerable<byte[]> messages)
        {
            ValidateTopic(topic);
            if (messages == null)
                throw new ArgumentNullException("messages");

            ICollection<byte[]> msgList = messages as ICollection<byte[]> ?? messages.ToList();

            if (msgList.Count == 0)
                return null;

            byte[] body;
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    Binary.BigEndian.PutUint32(binaryWriter, msgList.Count);

                    foreach (var msg in msgList)
                    {
                        Binary.BigEndian.PutUint32(binaryWriter, msg.Length);
                        binaryWriter.Write(msg);
                    }
                }

                body = memoryStream.ToArray();
            }

            string route = string.Format("/mpub?topic={0}&binary=true", topic);
            return Post(route, body);
        }

        /// <summary>
        /// Empty a topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <returns>The response from the nsqd HTTP server.</returns>
        public string EmptyTopic(string topic)
        {
            ValidateTopic(topic);

            string route = string.Format("/topic/empty?topic={0}", topic);
            return Post(route);
        }

        /// <summary>
        /// Empty a channel.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The response from the nsqd HTTP server.</returns>
        public string EmptyChannel(string topic, string channel)
        {
            ValidateTopicAndChannel(topic, channel);

            string route = string.Format("/channel/empty?topic={0}&channel={1}", topic, channel);
            return Post(route);
        }

        /// <summary>
        /// Pause a topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <returns>The response from the nsqd HTTP server.</returns>
        public string PauseTopic(string topic)
        {
            ValidateTopic(topic);

            string route = string.Format("/topic/pause?topic={0}", topic);
            return Post(route);
        }

        /// <summary>
        /// Unpause a topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <returns>The response from the nsqd HTTP server.</returns>
        public string UnpauseTopic(string topic)
        {
            ValidateTopic(topic);

            string route = string.Format("/topic/unpause?topic={0}", topic);
            return Post(route);
        }

        /// <summary>
        /// Pause a channel.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The response from the nsqd HTTP server.</returns>
        public string PauseChannel(string topic, string channel)
        {
            ValidateTopicAndChannel(topic, channel);

            string route = string.Format("/channel/pause?topic={0}&channel={1}", topic, channel);
            return Post(route);
        }

        /// <summary>
        /// Unpause a channel.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>The response from the nsqd HTTP server.</returns>
        public string UnpauseChannel(string topic, string channel)
        {
            ValidateTopicAndChannel(topic, channel);

            string route = string.Format("/channel/unpause?topic={0}&channel={1}", topic, channel);
            return Post(route);
        }

        /// <summary>
        /// Returns internal instrumented statistics.
        /// </summary>
        /// <returns>The response from the nsqd HTTP server.</returns>
        public NsqdStats GetStats()
        {
            string endpoint = GetFullUrl("/stats?format=json");
            byte[] respBody = Request(endpoint, HttpMethod.Get, _timeoutMilliseconds);

            var serializer = new DataContractJsonSerializer(typeof(NsqdStats));
            using (var memoryStream = new MemoryStream(respBody))
            {
                return ((NsqdStats)serializer.ReadObject(memoryStream));
            }
        }
    }
}
