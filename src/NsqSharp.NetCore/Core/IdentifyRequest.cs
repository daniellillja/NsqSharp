using System.Runtime.Serialization;

namespace NsqSharp.Core
{
    /// <summary>
    /// Identify request.
    /// </summary>
    [DataContract]
    public class IdentifyRequest
    {
        /// <summary>client_id</summary>
        [DataMember(Name = "client_id")]
        public string client_id { get; set; }
        /// <summary>hostname</summary>
        [DataMember(Name = "hostname")]
        public string hostname { get; set; }
        /// <summary>user_agent</summary>
        [DataMember(Name = "user_agent")]
        public string user_agent { get; set; }
        /// <summary>short_id (deprecated)</summary>
        [DataMember(Name = "short_id")]
        public string short_id { get; set; }
        /// <summary>long_id (deprecated)</summary>
        [DataMember(Name = "long_id")]
        public string long_id { get; set; }
        /// <summary>tls_v1</summary>
        [DataMember(Name = "tls_v1")]
        public bool tls_v1 { get; set; }
        /// <summary>deflate</summary>
        [DataMember(Name = "deflate")]
        public bool deflate { get; set; }
        /// <summary>deflate_level</summary>
        [DataMember(Name = "deflate_level")]
        public int deflate_level { get; set; }
        /// <summary>snappy</summary>
        [DataMember(Name = "snappy")]
        public bool snappy { get; set; }
        /// <summary>feature_negotiation</summary>
        [DataMember(Name = "feature_negotiation")]
        public bool feature_negotiation { get; set; }
        /// <summary>heartbeat_interval</summary>
        [DataMember(Name = "heartbeat_interval")]
        public int heartbeat_interval { get; set; }
        /// <summary>sample_rate</summary>
        [DataMember(Name = "sample_rate")]
        public int sample_rate { get; set; }
        /// <summary>output_buffer_size</summary>
        [DataMember(Name = "output_buffer_size")]
        public long output_buffer_size { get; set; }
        /// <summary>output_buffer_timeout</summary>
        [DataMember(Name = "output_buffer_timeout")]
        public int output_buffer_timeout { get; set; }
        /// <summary>msg_timeout</summary>
        [DataMember(Name = "msg_timeout")]
        public int msg_timeout { get; set; }
    }
}