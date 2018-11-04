using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ProArch.CodingTest.Common.Exception
{
    public class ExternalInvoiceServiceException : ApplicationException
    {
        public ExternalInvoiceServiceException()
        {
        }

        public ExternalInvoiceServiceException(string message) : base(message)
        {
        }

        public ExternalInvoiceServiceException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected ExternalInvoiceServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
