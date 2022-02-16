using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AD.CAAPS.EmailServices
{
    public class EmailConfigurationException : Exception
    {
        public EmailConfigurationException()
        {
        }

        public EmailConfigurationException(string message) : base(message)
        {
        }

        public EmailConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmailConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
