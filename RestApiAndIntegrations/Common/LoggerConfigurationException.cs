using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AD.CAAPS.Common
{
    [Serializable]
    public class LoggerConfigurationException : ConfigurationException
    {
        public LoggerConfigurationException() { }
        public LoggerConfigurationException(string message) : base(message) { }
        public LoggerConfigurationException(string message, Exception innerException) : base(message, innerException) { }
        protected LoggerConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
