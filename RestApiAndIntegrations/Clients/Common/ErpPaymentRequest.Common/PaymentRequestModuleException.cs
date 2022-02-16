using System;
using System.Runtime.Serialization;

namespace AD.CAAPS.ErpPaymentRequest.Common
{
  [Serializable]
  public class PaymentRequestModuleException : Exception
  {
    public PaymentRequestModuleException() { }
    public PaymentRequestModuleException(string message) : base(message) { }
    public PaymentRequestModuleException(string message, Exception innerException) : base(message, innerException) { }
    protected PaymentRequestModuleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
