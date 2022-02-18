using System;
using System.Runtime.Serialization;

namespace AD.CAAPS.Common
{
    [Serializable]
    public class MoveFileException : Exception
    {
        public MoveFileException()
        {
        }

        public MoveFileException(string message) : base(message)
        {
        }

        public MoveFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MoveFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}