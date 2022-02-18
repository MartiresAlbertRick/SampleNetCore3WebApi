using System;
using System.Runtime.Serialization;

namespace AD.CAAPS.Importer.Logic
{
    [Serializable]
    public class DvDataImportConfigurationError : Exception
    {
        public DvDataImportConfigurationError()
        {
        }

        public DvDataImportConfigurationError(string message) : base(message)
        {
        }

        public DvDataImportConfigurationError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DvDataImportConfigurationError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}