using System.Runtime.Serialization;

namespace NsqSharp.Api
{
    /// <summary>nsqlookupd response to /topics.</summary>
    [DataContract]
    public class NsqLookupdTopicsResponse
    {
        /// <summary>Gets or sets the topics.</summary>
        /// <value>The topics.</value>
        [DataMember(Name = "topics")]
        public string[] Topics { get; set; }
    }
}