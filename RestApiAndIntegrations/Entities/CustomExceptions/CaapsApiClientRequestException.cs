using System;

namespace AD.CAAPS.Entities.CustomExceptions
{
    public class CaapsApiClientRequestException : Exception
    {
        public CaapsApiClientRequestException() { }
        public CaapsApiClientRequestException(string message) : base(message) { }
        public CaapsApiClientRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}