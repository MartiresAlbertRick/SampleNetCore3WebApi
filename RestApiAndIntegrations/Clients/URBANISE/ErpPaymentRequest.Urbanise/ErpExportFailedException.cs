using System;
using System.Runtime.Serialization;

namespace AD.CAAPS.ErpPaymentRequest.Urbanise
{
    [Serializable]
    public class ErpExportFailedException : Exception
    {
        public ErpExportFailedException()
        {
        }

        public ErpExportFailedException(string message) : base(message)
        {
        }

        public ErpExportFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ErpExportFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}