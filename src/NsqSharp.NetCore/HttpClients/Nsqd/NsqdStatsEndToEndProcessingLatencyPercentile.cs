using System;
using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary></summary>
    [DataContract]
    public class NsqdStatsEndToEndProcessingLatencyPercentile
    {
        /// <summary>quantile</summary>
        [DataMember(Name = "quantile")]
        public double Quantile { get; set; }
        /// <summary>value</summary>
        [DataMember(Name = "value")]
        public long Value { get; set; }
        /// <summary>time</summary>
        public TimeSpan Time
        {
            get { return TimeSpan.FromSeconds(Value / 1000000000.0); }
        }
    }
}