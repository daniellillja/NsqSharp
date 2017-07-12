using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary>
    /// Client information for nsqd. See <see cref="NsqdHttpClient.GetStats"/>.
    /// </summary>
    [DataContract]
    public class NsqdStatsClient
    {
        ///<summary>name</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }
        ///<summary>client_id</summary>
        [DataMember(Name = "client_id")]
        public string ClientId { get; set; }
        ///<summary>hostname</summary>
        [DataMember(Name = "hostname")]
        public string Hostname { get; set; }
        ///<summary>version</summary>
        [DataMember(Name = "version")]
        public string Version { get; set; }
        ///<summary>remote_address</summary>
        [DataMember(Name = "remote_address")]
        public string RemoteAddress { get; set; }
        ///<summary>state</summary>
        [DataMember(Name = "state")]
        public int State { get; set; }
        ///<summary>ready_count</summary>
        [DataMember(Name = "ready_count")]
        public int ReadyCount { get; set; }
        ///<summary>in_flight_count</summary>
        [DataMember(Name = "in_flight_count")]
        public int InFlightCount { get; set; }
        ///<summary>message_count</summary>
        [DataMember(Name = "message_count")]
        public int MessageCount { get; set; }
        ///<summary>finish_count</summary>
        [DataMember(Name = "finish_count")]
        public int FinishCount { get; set; }
        ///<summary>requeue_count</summary>
        [DataMember(Name = "requeue_count")]
        public int RequeueCount { get; set; }
        ///<summary>connect_ts</summary>
        [DataMember(Name = "connect_ts")]
        public int ConnectTimestamp { get; set; }
        ///<summary>sample_rate</summary>
        [DataMember(Name = "sample_rate")]
        public int SampleRate { get; set; }
        ///<summary>deflate</summary>
        [DataMember(Name = "deflate")]
        public bool Deflate { get; set; }
        ///<summary>snappy</summary>
        [DataMember(Name = "snappy")]
        public bool Snappy { get; set; }
        ///<summary>user_agent</summary>
        [DataMember(Name = "user_agent")]
        public string UserAgent { get; set; }
        ///<summary>tls</summary>
        [DataMember(Name = "tls")]
        public bool Tls { get; set; }
        ///<summary>tls_cipher_suite</summary>
        [DataMember(Name = "tls_cipher_suite")]
        public string TlsCipherSuite { get; set; }
        ///<summary>tls_version</summary>
        [DataMember(Name = "tls_version")]
        public string TlsVersion { get; set; }
        ///<summary>tls_negotiated_protocol</summary>
        [DataMember(Name = "tls_negotiated_protocol")]
        public string TlsNegotiatedProtocol { get; set; }
        ///<summary>tls_negotiated_protocol_is_mutual</summary>
        [DataMember(Name = "tls_negotiated_protocol_is_mutual")]
        public bool TlsNegotiatedProtocolIsMutual { get; set; }
    }
}