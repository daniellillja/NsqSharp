using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary>
    /// Channel information for nsqd. See <see cref="NsqdHttpClient.GetStats"/>.
    /// </summary>
    [DataContract]
    public class NsqdStatsChannel
    {
        ///<summary>channel_name</summary>
        [DataMember(Name = "channel_name")]
        public string ChannelName { get; set; }
        ///<summary>depth</summary>
        [DataMember(Name = "depth")]
        public int Depth { get; set; }
        ///<summary>backend_depth</summary>
        [DataMember(Name = "backend_depth")]
        public int BackendDepth { get; set; }
        ///<summary>in_flight_count</summary>
        [DataMember(Name = "in_flight_count")]
        public int InFlightCount { get; set; }
        ///<summary>deferred_count</summary>
        [DataMember(Name = "deferred_count")]
        public int DeferredCount { get; set; }
        ///<summary>message_count</summary>
        [DataMember(Name = "message_count")]
        public int MessageCount { get; set; }
        ///<summary>requeue_count</summary>
        [DataMember(Name = "requeue_count")]
        public int RequeueCount { get; set; }
        ///<summary>timeout_count</summary>
        [DataMember(Name = "timeout_count")]
        public int TimeoutCount { get; set; }
        ///<summary>clients</summary>
        [DataMember(Name = "clients")]
        public NsqdStatsClient[] Clients { get; set; }
        ///<summary>paused</summary>
        [DataMember(Name = "paused")]
        public bool Paused { get; set; }
        // TODO
        //[DataMember(Name = "e2e_processing_latency")]
        //public NsqdStatsEndToEndProcessingLatency EndToEndProcessingLatency { get; set; }
    }
}