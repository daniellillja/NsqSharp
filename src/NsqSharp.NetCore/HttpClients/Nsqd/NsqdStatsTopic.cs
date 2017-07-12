using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary>
    /// Topic information for nsqd. See <see cref="NsqdHttpClient.GetStats"/>.
    /// </summary>
    [DataContract]
    public class NsqdStatsTopic
    {
        ///<summary>topic_name</summary>
        [DataMember(Name = "topic_name")]
        public string TopicName { get; set; }
        ///<summary>channels</summary>
        [DataMember(Name = "channels")]
        public NsqdStatsChannel[] Channels { get; set; }
        ///<summary>depth</summary>
        [DataMember(Name = "depth")]
        public int Depth { get; set; }
        ///<summary>backend_depth</summary>
        [DataMember(Name = "backend_depth")]
        public int BackendDepth { get; set; }
        ///<summary>message_count</summary>
        [DataMember(Name = "message_count")]
        public int MessageCount { get; set; }
        ///<summary>paused</summary>
        [DataMember(Name = "paused")]
        public bool Paused { get; set; }
        ///<summary>e2e_processing_latency</summary>
        [DataMember(Name = "e2e_processing_latency")]
        public NsqdStatsEndToEndProcessingLatency EndToEndProcessingLatency { get; set; }
    }
}