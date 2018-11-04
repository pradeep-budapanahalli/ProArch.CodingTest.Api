using ProArch.CodingTest.Common.Models;
using ProArch.CodingTest.Common.Exception;
using ProArch.CodingTest.Common.Services;
using ProArch.CodingTest.External;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProArch.CodingTest.Services
{
    public class ExternalInvoiceService : IInvoiceService
    {
        public InvoiceServiceCategory ServiceType => InvoiceServiceCategory.External;

        public IEnumerable<SpendDetail> GetSpendDetails(int supplierId)
        {
            try
            {
                return External.ExternalInvoiceService.GetInvoices(supplierId.ToString())?.Select(ivc => new SpendDetail()
                {
                    TotalSpend = ivc.TotalAmount,
                    Year = ivc.Year
                });
            }
            catch (Exception ex)
            {
                throw new ExternalInvoiceServiceException("External Invoice Service Failed.", ex);
            }
        }
    }
}
