using System;

namespace AD.CAAPS.Importer.Common
{
    public class InvalidImportTypeException : Exception
    {
        public InvalidImportTypeException() { }
        public InvalidImportTypeException(string message) : base(message) { }
        public InvalidImportTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
