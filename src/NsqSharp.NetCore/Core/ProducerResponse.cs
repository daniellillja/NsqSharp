using System;
using NsqSharp.Core;
using NsqSharp.Utils.Channels;

namespace NsqSharp
{
    /// <summary>
    /// ProducerResponse is returned by the async publish methods
    /// to retrieve metadata about the command after the
    /// response is received.
    /// </summary>
    public class ProducerResponse
    {
        internal Command _cmd;
        internal Chan<ProducerResponse> _doneChan;

        /// <summary>
        /// the error (or nil) of the publish command
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// the slice of variadic arguments passed to PublishAsync or MultiPublishAsync
        /// </summary>
        public object[] Args { get; set; }

        internal void finish()
        {
            if (_doneChan != null)
                _doneChan.Send(this);
        }
    }
}