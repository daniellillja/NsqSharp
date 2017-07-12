using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary>
    /// Statistics information for nsqd. See <see cref="NsqdHttpClient.GetStats"/>.
    /// </summary>
    [DataContract]
    public class NsqdStats
    {
        ///<summary>version</summary>
        [DataMember(Name = "version")]
        public string Version { get; set; }
        ///<summary>health</summary>
        [DataMember(Name = "health")]
        public string Health { get; set; }
        ///<summary>topics</summary>
        [DataMember(Name = "topics")]
        public NsqdStatsTopic[] Topics { get; set; }
    }
}