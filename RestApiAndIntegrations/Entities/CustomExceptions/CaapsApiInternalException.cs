using System;

namespace AD.CAAPS.Entities.CustomExceptions
{
    public class CaapsApiInternalException : Exception
    {
        public CaapsApiInternalException() { }
        public CaapsApiInternalException(string message) : base(message) { }
        public CaapsApiInternalException(string message, Exception innerException) : base(message, innerException) { }
    }
}