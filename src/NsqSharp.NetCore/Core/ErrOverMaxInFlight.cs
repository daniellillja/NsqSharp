using System;

namespace NsqSharp.Core
{
    /// <summary>
    /// ErrOverMaxInFlight is returned from Consumer if over max-in-flight
    /// </summary>
    //[Serializable]
    public class ErrOverMaxInFlight : Exception
    {
        /// <summary>Initializes a new instance of the ErrOverMaxInFlight class.</summary>
        public ErrOverMaxInFlight()
            : base("over configured max-inflight")
        {
            // TODO: go-nsq PR: fix typo "over configure"
            // https://github.com/nsqio/go-nsq/blob/08a850b52c79a9a1b6e457233bd11bf7ba713178/errors.go#L23
        }

        /* /// <summary>
         /// Initializes a new instance of the <see cref="ErrOverMaxInFlight"/> class with serialized data.
         /// </summary>
         /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about
         /// the exception being thrown.</param>
         /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about
         /// the source or destination.</param>
         /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is null.</exception>
         /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/>
         /// is zero (0).</exception>
         protected ErrOverMaxInFlight(SerializationInfo info, StreamingContext context)
             : base(info, context)
         {
         }*/
    }
}