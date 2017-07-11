using System.Runtime.Serialization;

namespace NsqSharp.Core
{
    /// <summary>
    /// IdentifyResponse represents the metadata
    /// returned from an IDENTIFY command to nsqd
    /// </summary>
    [DataContract]
    public class IdentifyResponse
    {
        /// <summary>Max RDY count</summary>
        [DataMember(Name = "max_rdy_count")]
        public long MaxRdyCount { get; set; }
        /// <summary>Use TLSv1</summary>
        [DataMember(Name = "tls_v1")]
        public bool TLSv1 { get; set; }
        /// <summary>Use Deflate compression</summary>
        [DataMember(Name = "deflate")]
        public bool Deflate { get; set; }
        /// <summary>Use Snappy compression</summary>
        [DataMember(Name = "snappy")]
        public bool Snappy { get; set; }
        /// <summary>Auth required</summary>
        [DataMember(Name = "auth_required")]
        public bool AuthRequired { get; set; }
    }
}