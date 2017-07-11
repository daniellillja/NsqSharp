using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary>nsqlookupd producer list from /lookup?topic=[topic_name].</summary>
    [DataContract]
    public class TopicProducerInformation
    {
        /// <summary>Gets or sets the remote address.</summary>
        /// <value>The remote address.</value>
        [DataMember(Name = "remote_address")]
        public string RemoteAddress { get; set; }

        /// <summary>Gets or sets the hostname.</summary>
        /// <value>The hostname.</value>
        [DataMember(Name = "hostname")]
        public string Hostname { get; set; }

        /// <summary>Gets or sets the broadcast address.</summary>
        /// <value>The broadcast address.</value>
        [DataMember(Name = "broadcast_address")]
        public string BroadcastAddress { get; set; }

        /// <summary>Gets or sets the TCP port.</summary>
        /// <value>The TCP port.</value>
        [DataMember(Name = "tcp_port")]
        public int TcpPort { get; set; }

        /// <summary>Gets or sets the HTTP port.</summary>
        /// <value>The HTTP port.</value>
        [DataMember(Name = "http_port")]
        public int HttpPort { get; set; }

        /// <summary>Gets or sets the nsqd version.</summary>
        /// <value>The nsqd version.</value>
        [DataMember(Name = "version")]
        public string Version { get; set; }
    }
}