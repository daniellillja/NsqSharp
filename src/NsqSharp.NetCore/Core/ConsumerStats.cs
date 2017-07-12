namespace NsqSharp
{
    /// <summary>
    ///     <see cref="ConsumerStats" /> represents a snapshot of the state of a <see cref="Consumer"/>'s connections and the
    ///     messages it has seen.
    /// </summary>
    public class ConsumerStats
    {
        /// <summary>The number of messages received.</summary>
        /// <value>The number of messages received.</value>
        public long MessagesReceived { get; internal set; }

        /// <summary>The number of messages finished.</summary>
        /// <value>The number of messages finished.</value>
        public long MessagesFinished { get; internal set; }

        /// <summary>The number of messages requeued.</summary>
        /// <value>The number of messages requeued.</value>
        public long MessagesRequeued { get; internal set; }

        /// <summary>The number of nsqd connections.</summary>
        /// <value>The number of nsqd connections.</value>
        public int Connections { get; internal set; }
    }
}