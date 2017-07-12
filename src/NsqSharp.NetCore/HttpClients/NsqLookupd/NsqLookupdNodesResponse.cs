using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary>nsqlookupd response from /nodes.</summary>
    [DataContract]
    public class NsqLookupdNodesResponse
    {
        /// <summary>Gets or sets the producers.</summary>
        /// <value>The producers.</value>
        [DataMember(Name = "producers")]
        public ProducerInformation[] Producers { get; set; }
    }
}