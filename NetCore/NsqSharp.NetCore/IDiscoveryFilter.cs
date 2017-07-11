using System.Collections.Generic;

namespace NsqSharp
{
    /// <summary>
    ///     <see cref="IDiscoveryFilter" /> is accepted by <see cref="Consumer.SetBehaviorDelegate"/>
    ///     for filtering the nsqd addresses returned from nsqlookupd.
    /// </summary>
    public interface IDiscoveryFilter
    {
        /// <summary>Filters a list of nsqd addresses.</summary>
        /// <param name="nsqds">nsqd addresses returned by nsqlookupd.</param>
        /// <returns>The filtered list of nsqd addresses to use.</returns>
        IEnumerable<string> Filter(IEnumerable<string> nsqds);
    }
}