using System.Runtime.Serialization;

namespace NsqSharp.Core
{
    /// <summary>
    /// AuthResponse represents the metadata
    /// returned from an AUTH command to nsqd
    /// </summary>
    [DataContract]
    public class AuthResponse
    {
        /// <summary>Identity</summary>
        [DataMember(Name = "identity")]
        public string Identity { get; set; }
        /// <summary>Identity URL</summary>
        [DataMember(Name = "identity_url")]
        public string IdentityUrl { get; set; }
        /// <summary>Permission Count</summary>
        [DataMember(Name = "permission_count")]
        public long PermissionCount { get; set; }
    }
}