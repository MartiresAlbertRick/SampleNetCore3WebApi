using System;
using System.Runtime.Serialization;

namespace AD.CAAPS.Importer.Logic
{
    [Serializable]
    public class FileReaderError : Exception
    {
        public FileReaderError()
        {
        }

        public FileReaderError(Exception innerException) : base( message: "FailedReaderError", innerException )
        {
        }

        public FileReaderError(string message) : base(message)
        {
        }

        public FileReaderError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FileReaderError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}