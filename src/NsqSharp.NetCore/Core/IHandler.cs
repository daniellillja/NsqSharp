namespace NsqSharp
{
    /// <summary>
    ///     <para>Message processing interface for <see cref="Consumer" />.</para>
    ///     <para>When the <see cref="HandleMessage"/> method returns the <see cref="Consumer"/> will automatically handle
    ///     FIN'ing the message.</para>
    ///     <para>When an exception is thrown the <see cref="Consumer"/> will automatically handle REQ'ing the message.</para>
    /// </summary>
    /// <seealso cref="Consumer.AddHandler"/>
    public interface IHandler
    {
        /// <summary>Handles a message.</summary>
        /// <param name="message">The message.</param>
        void HandleMessage(IMessage message);

        /// <summary>
        ///     Called when a <see cref="Message"/> has exceeded the <see cref="Consumer"/> specified
        ///     <see cref="NsqConfig.MaxAttempts"/>.
        /// </summary>
        /// <param name="message">The failed message.</param>
        void LogFailedMessage(IMessage message);
    }
}