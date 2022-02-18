using System;
using System.Runtime.Serialization;

namespace AD.CAAPS.Common
{
    [Serializable]
    public class SystemOptionMissingException : ConfigurationException
    {
        public SystemOptionMissingException() { }
        public SystemOptionMissingException(string message) : base(message) { }
        public SystemOptionMissingException(string message, Exception innerException) : base(message, innerException) { }
        protected SystemOptionMissingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
