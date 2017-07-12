using System;

namespace NsqSharp.Core
{
    /// <summary>
    /// ErrClosing is returned when a connection is closing
    /// </summary>
    //[Serializable]
    public class ErrClosing : Exception
    {
        /// <summary>Initializes a new instance of the ErrClosing class.</summary>
        public ErrClosing()
            : base("closing")
        {
        }

        /*    /// <summary>
            /// Initializes a new instance of the <see cref="ErrClosing"/> class with serialized data.
            /// </summary>
            /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about
            /// the exception being thrown.</param>
            /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about
            /// the source or destination.</param>
            /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is null.</exception>
            /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/>
            /// is zero (0).</exception>
            protected ErrClosing(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }*/
    }
}