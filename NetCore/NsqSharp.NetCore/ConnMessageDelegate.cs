using System;

namespace NsqSharp.Core
{
    // https://github.com/nsqio/go-nsq/blob/master/delegates.go

    internal class ConnMessageDelegate : IMessageDelegate
    {
        public Conn c { get; set; }

        public void OnFinish(Message m) { c.onMessageFinish(m); }
        public TimeSpan OnRequeue(Message m, TimeSpan? delay, bool backoff)
        {
            return c.onMessageRequeue(m, delay, backoff);
        }
        public void OnTouch(Message m) { c.onMessageTouch(m); }
    }
}
