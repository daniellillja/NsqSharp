using System;

namespace NsqSharp.Core
{
    /// <summary>
    /// ErrProtocol is returned from Producer when encountering
    /// an NSQ protocol level error
    /// </summary>
    //[Serializable]
    public class ErrProtocol : Exception
    {
        /// <summary>Initializes a new instance of the ErrProtocol class.</summary>
        public ErrProtocol(string reason)
            : base(reason)
        {
        }

        /* /// <summary>
         /// Initializes a new instance of the <see cref="ErrProtocol"/> class with serialized data.
         /// </summary>
         /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about
         /// the exception being thrown.</param>
         /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about
         /// the source or destination.</param>
         /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is null.</exception>
         /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/>
         /// is zero (0).</exception>
         protected ErrProtocol(SerializationInfo info, StreamingContext context)
             : base(info, context)
         {
         }*/
    }
}