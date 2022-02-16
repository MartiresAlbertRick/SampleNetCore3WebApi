using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace AD.CAAPS.Common
{
    [Serializable]
    public class ConfigurationReadException : ConfigurationException
    {
        public ConfigurationReadException() { }
        public ConfigurationReadException(string message) : base(message) { }
        public ConfigurationReadException(string message, Exception innerException) : base(message, innerException) { }
        protected ConfigurationReadException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
