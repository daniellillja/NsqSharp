using System;
using System.IO;

namespace NsqSharp
{
    /// <summary>
    ///     Message is the fundamental data type containing the <see cref="Id"/>, <see cref="Body"/>, and metadata of a
    ///     message received from an nsqd instance.
    /// </summary>
    public interface IMessage
    {
        /// <summary>The message body byte array.</summary>
        /// <value>The message body byte array.</value>
        byte[] Body { get; }

        /// <summary>The original timestamp when the message was published.</summary>
        /// <value>The original timestamp when the message was published.</value>
        DateTime Timestamp { get; }

        /// <summary>The current attempt count to process this message. The first attempt is <c>1</c>.</summary>
        /// <value>The current attempt count to process this message. The first attempt is <c>1</c>.</value>
        int Attempts { get; }

        /// <summary>The maximum number of attempts before nsqd will permanently fail this message.</summary>
        /// <value>The maximum number of attempts before nsqd will permanently fail this message.</value>
        int MaxAttempts { get; }

        /// <summary>The nsqd address which sent this message.</summary>
        /// <value>The nsqd address which sent this message.</value>
        string NsqdAddress { get; }

        /// <summary>
        ///     <para>Disables the automatic response that would normally be sent when <see cref="IHandler.HandleMessage"/>
        ///     returns or throws.</para>
        ///     
        ///     <para>This is useful if you want to batch, buffer, or asynchronously respond to messages.</para>
        /// </summary>
        void DisableAutoResponse();

        /// <summary>
        ///     Indicates whether or not this message will be responded to automatically when
        ///     <see cref="IHandler.HandleMessage"/> returns or throws.
        /// </summary>
        /// <value><c>true</c> if automatic response is disabled; otherwise, <c>false</c>.</value>
        bool IsAutoResponseDisabled { get; }

        /// <summary>Indicates whether or not this message has been FIN'd or REQ'd.</summary>
        /// <value><c>true</c> if this message has been FIN'd or REQ'd; otherwise, <c>false</c>.</value>
        bool HasResponded { get; }

        /// <summary>
        ///     Sends a FIN command to the nsqd which sent this message, indicating the message processed successfully.
        /// </summary>
        void Finish();

        /// <summary>
        ///     <para>Sends a TOUCH command to the nsqd which sent this message, resetting the message timeout.</para>
        ///     
        ///     <para>The nsqd default "-msg-timeout" is 60s. See <see cref="NsqConfig.MessageTimeout"/>.</para>
        /// 
        ///     <para>
        ///           nsqd will requeue the message, regardless of calls to TOUCH, if the time exceeds the nsqd option
        ///           set for "-max-msg-timeout" (nsqd default = 15m).
        ///     </para>
        ///     
        ///     <para>If FIN or REQ have already been sent for this message, calling <see cref="Touch"/> has no effect.</para>
        /// </summary>
        void Touch();

        /// <summary>
        ///     <para>Sends a REQ command to the nsqd which sent this message, using the supplied delay.</para>
        ///     
        ///     <para>A delay of <c>null</c> will automatically calculate based on the number of attempts and the configured
        ///     <see cref="NsqConfig.DefaultRequeueDelay"/>.</para>
        ///     
        ///     <para>Using this method to respond triggers a backoff event.</para>
        /// </summary>
        /// <param name="delay">The minimum amount of time the message will be requeued.</param>
        void Requeue(TimeSpan? delay = null);

        /// <summary>
        ///     <para>Sends a REQ command to the nsqd which sent this message, using the supplied delay.</para>
        /// 
        ///     <para>A delay of <c>null</c> will automatically calculate based on the number of attempts and the configured
        ///     <see cref="NsqConfig.DefaultRequeueDelay"/>.</para>
        ///     
        ///     <para>Using this method to respond does not trigger a backoff event.</para>
        /// </summary>
        /// <param name="delay">The minimum amount of time the message will be requeued.</param>
        void RequeueWithoutBackoff(TimeSpan? delay);

        /// <summary>
        ///     Indicates whether this message triggered a backoff event, causing the <see cref="Consumer"/>
        ///     to slow its processing based on <see cref="NsqConfig.BackoffStrategy"/>.
        /// </summary>
        /// <value><c>true</c> if this message triggered a backoff event; otherwise, <c>false</c>.</value>
        bool BackoffTriggered { get; }

        /// <summary>
        ///     The minimum date/time the message will be requeued until; <c>null</c> indicates the message has not been
        ///     requeued.
        /// </summary>
        /// <value>
        ///     The minimum date/time the message will be requeued until; <c>null</c> indicates the message has not been
        ///     requeued.
        /// </value>
        DateTime? RequeuedUntil { get; }

        /// <summary>
        ///     <para>Encodes the message frame and body and writes it to the supplied <paramref name="writeStream"/>.</para>
        ///     
        ///     <para>It is suggested that the target <paramref name="writeStream"/> is buffered to avoid performing many
        ///     system calls.</para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="writeStream"/> is <c>null</c>.</exception>
        /// <param name="writeStream">The stream to write this message.</param>
        /// <returns>The number of bytes written to <paramref name="writeStream"/>.</returns>
        Int64 WriteTo(Stream writeStream);

        /// <summary>
        ///     <para>The message ID as a hexadecimal string.</para>
        ///     
        ///     <para>The message ID for a given message will be the same across channels; the message ID is created at the
        ///     topic level. If the message is requeued or times out it will retain the same message ID on future
        ///     attempts.</para>
        /// </summary>
        /// <value>The message ID as a hexadecimal string.</value>
        string Id { get; }
    }
}