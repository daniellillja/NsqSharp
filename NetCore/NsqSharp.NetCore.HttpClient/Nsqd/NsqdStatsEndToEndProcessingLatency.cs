using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary></summary>
    [DataContract]
    public class NsqdStatsEndToEndProcessingLatency
    {
        /// <summary>count</summary>
        [DataMember(Name = "count")]
        public int Count { get; set; }
        /// <summary>percentiles</summary>
        [DataMember(Name = "percentiles")]
        public NsqdStatsEndToEndProcessingLatencyPercentile[] Percentiles { get; set; }
    }
}