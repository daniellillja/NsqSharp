using System;

namespace NsqSharp.Core
{
    /// <summary>
    /// ErrStopped is returned when a publish command is
    /// made against a Producer that has been stopped 
    /// </summary>
    //[Serializable]
    public class ErrStopped : Exception
    {
        /// <summary>Initializes a new instance of the ErrStopped class.</summary>
        public ErrStopped()
            : base("stopped")
        {
        }

        /*   /// <summary>
           /// Initializes a new instance of the <see cref="ErrStopped"/> class with serialized data.
           /// </summary>
           /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about
           /// the exception being thrown.</param>
           /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about
           /// the source or destination.</param>
           /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is null.</exception>
           /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/>
           /// is zero (0).</exception>
           protected ErrStopped(SerializationInfo info, StreamingContext context)
               : base(info, context)
           {
           }*/
    }
}