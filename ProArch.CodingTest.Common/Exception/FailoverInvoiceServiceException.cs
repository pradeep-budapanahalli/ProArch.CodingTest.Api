using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ProArch.CodingTest.Common.Exception
{
    public class FailoverInvoiceServiceException : ApplicationException
    {
        public FailoverInvoiceServiceException()
        {
        }

        public FailoverInvoiceServiceException(string message) : base(message)
        {
        }

        public FailoverInvoiceServiceException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected FailoverInvoiceServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
