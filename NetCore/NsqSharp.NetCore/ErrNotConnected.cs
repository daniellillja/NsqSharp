using System;

namespace NsqSharp.Core
{
    // https://github.com/nsqio/go-nsq/blob/master/errors.go

    /// <summary>
    /// ErrNotConnected is returned when a publish command is made
    /// against a Producer that is not connected
    /// </summary>
    //[Serializable]
    public class ErrNotConnected : Exception
    {
        /// <summary>Initializes a new instance of the ErrNotConnected class.</summary>
        public ErrNotConnected()
            : base("not connected")
        {
        }

        /*  /// <summary>
          /// Initializes a new instance of the <see cref="ErrNotConnected"/> class with serialized data.
          /// </summary>
          /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about
          /// the exception being thrown.</param>
          /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about
          /// the source or destination.</param>
          /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is null.</exception>
          /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/>
          /// is zero (0).</exception>
          protected ErrNotConnected(SerializationInfo info, StreamingContext context)
              : base(info, context)
          {
          }*/
    }
}
