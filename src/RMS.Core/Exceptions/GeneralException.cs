using System;
using System.Runtime.Serialization;

namespace RMS.Core
{
    /// <inheritdoc />
    /// <summary>
    /// The custom exception for validation errors
    /// </summary>
    [Serializable]
    public class GeneralException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Create new instance of Application validation error exception
        /// </summary>
        public GeneralException(string message) : base(message)
        {
        }

        public GeneralException(string messageFormat, params object[] args)
            : this(string.Format(messageFormat, args))
        {
        }

        protected GeneralException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public GeneralException()
        {
        }

        public GeneralException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
