using System;

namespace NsqSharp.Core
{
    /// <summary>
    /// ConnDelegate is an interface of methods that are used as
    /// callbacks in Conn
    /// </summary>
    public interface IConnDelegate
    {
        /// <summary>
        /// OnResponse is called when the connection
        /// receives a FrameTypeResponse from nsqd
        /// </summary>
        void OnResponse(Conn c, byte[] data);

        /// <summary>
        /// OnError is called when the connection
        /// receives a FrameTypeError from nsqd
        /// </summary>
        void OnError(Conn c, byte[] data);

        /// <summary>
        /// OnMessage is called when the connection
        /// receives a FrameTypeMessage from nsqd
        /// </summary>
        void OnMessage(Conn c, Message m);

        /// <summary>
        /// OnMessageFinished is called when the connection
        /// handles a FIN command from a message handler
        /// </summary>
        void OnMessageFinished(Conn c, Message m);

        /// <summary>
        /// OnMessageRequeued is called when the connection
        /// handles a REQ command from a message handler
        /// </summary>
        void OnMessageRequeued(Conn c, Message m);

        /// <summary>
        /// OnBackoff is called when the connection triggers a backoff state
        /// </summary>
        void OnBackoff(Conn c);

        /// <summary>
        /// OnContinue is called when the connection finishes a message without adjusting backoff state
        /// </summary>
        void OnContinue(Conn c);

        /// <summary>
        /// OnResume is called when the connection triggers a resume state
        /// </summary>
        void OnResume(Conn c);

        /// <summary>
        /// OnIOError is called when the connection experiences
        /// a low-level TCP transport error
        /// </summary>
        void OnIOError(Conn c, Exception err);

        /// <summary>
        /// OnHeartbeat is called when the connection
        /// receives a heartbeat from nsqd
        /// </summary>
        void OnHeartbeat(Conn c);

        /// <summary>
        /// OnClose is called when the connection
        /// closes, after all cleanup
        /// </summary>
        void OnClose(Conn c);
    }
}