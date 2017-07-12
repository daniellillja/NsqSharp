using System;

namespace NsqSharp.Core
{
    /// <summary>
    /// MessageDelegate is an interface of methods that are used as
    /// callbacks in Message
    /// </summary>
    internal interface IMessageDelegate
    {
        /// <summary>
        /// OnFinish is called when the Finish() method
        /// is triggered on the Message
        /// </summary>
        void OnFinish(Message m);

        /// <summary>
        /// OnRequeue is called when the Requeue() method
        /// is triggered on the Message
        /// </summary>
        TimeSpan OnRequeue(Message m, TimeSpan? delay, bool backoff);

        /// <summary>
        /// OnTouch is called when the Touch() method
        /// is triggered on the Message
        /// </summary>
        void OnTouch(Message m);
    }
}