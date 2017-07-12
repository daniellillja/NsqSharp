using System;

namespace NsqSharp.Core
{
    /// <summary>
    /// ErrIdentify is returned from Conn as part of the IDENTIFY handshake
    /// </summary>
    //[Serializable]
    public class ErrIdentify : Exception
    {
        /// <summary>Initializes a new instance of the ErrIdentify class.</summary>
        public ErrIdentify(string reason)
            : this(reason, null)
        {
        }

        /// <summary>Initializes a new instance of the ErrIdentify class.</summary>
        public ErrIdentify(string reason, Exception innerException)
            : base(string.Format("failed to IDENTIFY - {0}", reason), innerException)
        {
            Reason = reason;
        }
        /*
                /// <summary>
                /// Initializes a new instance of the <see cref="ErrIdentify"/> class with serialized data.
                /// </summary>
                /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about
                /// the exception being thrown.</param>
                /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about
                /// the source or destination.</param>
                /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is null.</exception>
                /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/>
                /// is zero (0).</exception>
                protected ErrIdentify(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                    Reason = info.GetString("Reason");
                }

                /// <summary>
                /// Sets the <see cref="SerializationInfo"/> with information about the exception.
                /// </summary>
                /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception 
                /// being thrown.</param>
                /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or
                /// destination.</param>
                /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is a null reference.</exception>
                [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
                public override void GetObjectData(SerializationInfo info, StreamingContext context)
                {
                    if (info == null)
                        throw new ArgumentNullException("info");

                    info.AddValue("Reason", Reason);

                    base.GetObjectData(info, context);
                }*/

        /// <summary>
        /// Gets or sets the reason
        /// </summary>
        public string Reason { get; set; }
    }
}