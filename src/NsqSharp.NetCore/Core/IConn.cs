using NsqSharp.Core;

namespace NsqSharp
{
    /// <summary>
    /// IConn interface
    /// </summary>
    internal interface IConn
    {
        /// <summary>
        /// SetLogger assigns the logger to use as well as a level.
        ///
        /// The format parameter is expected to be a printf compatible string with
        /// a single {0} argument.  This is useful if you want to provide additional
        /// context to the log messages that the connection will print, the default
        /// is '({0})'.
        /// </summary>
        void SetLogger(ILogger l, string format);

        /// <summary>
        /// Connect dials and bootstraps the nsqd connection
        /// (including IDENTIFY) and returns the IdentifyResponse
        /// </summary>
        IdentifyResponse Connect();

        /// <summary>
        /// Close idempotently initiates connection close
        /// </summary>
        void Close();

        /// <summary>
        /// WriteCommand is a thread safe method to write a Command
        /// to this connection, and flush.
        /// </summary>
        void WriteCommand(Command command);
    }
}