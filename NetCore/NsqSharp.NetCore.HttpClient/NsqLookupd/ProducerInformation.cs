using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary>nsqlookupd producer list from /nodes.</summary>
    [DataContract]
    public class ProducerInformation : TopicProducerInformation
    {
        /// <summary>Gets a value indicating if an entry in <see cref="Topics"/> is tombstoned.</summary>
        /// <value>The tombstones.</value>
        [DataMember(Name = "tombstones")]
        public bool[] Tombstones { get; set; }

        /// <summary>Gets or sets the topics.</summary>
        /// <value>The topics.</value>
        [DataMember(Name = "topics")]
        public string[] Topics { get; set; }
    }
}