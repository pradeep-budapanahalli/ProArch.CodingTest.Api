using ProArch.CodingTest.Common.DTO;
using ProArch.CodingTest.Common.Exception;
using ProArch.CodingTest.Common.Services;
using ProArch.CodingTest.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProArch.CodingTest.Services
{
    public class FailoverInvoiceService : IInvoiceService
    {
        private int allowedDays;

        public FailoverInvoiceService(int allowedDays)
        {
            this.allowedDays = allowedDays;
        }

        public DateTime Timestamp { get; set; }
        public ExternalInvoice[] Invoices { get; set; }

        public InvoiceServiceType ServiceType => InvoiceServiceType.Failover;

        public IEnumerable<SpendDetail> GetSpendDetails(int supplierId)
        {
            if (DateTime.UtcNow.Subtract(Timestamp).TotalDays > allowedDays)
            {
                throw new FailoverInvoiceServiceException("FailoverInvoices expired");
            }
            return Invoices.Select(ivc => new SpendDetail()
            {
                TotalSpend = ivc.TotalAmount,
                Year = ivc.Year
            });
        }

    }
}
