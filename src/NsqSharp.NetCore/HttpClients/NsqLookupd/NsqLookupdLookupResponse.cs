using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary>nsqlookupd response from /lookup?topic=[topic_name].</summary>
    [DataContract]
    public class NsqLookupdLookupResponse
    {
        /// <summary>Gets or sets the nodes producing the topic.</summary>
        /// <value>The nodes producing the topic.</value>
        [DataMember(Name = "producers")]
        public TopicProducerInformation[] Producers { get; set; }

        /// <summary>Gets or sets the channels associated with the topic</summary>
        /// <value>The channels associated with the topic.</value>
        [DataMember(Name = "channels")]
        private string[] Channels { get; set; }
    }
}