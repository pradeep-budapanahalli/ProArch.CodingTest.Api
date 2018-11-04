using ProArch.CodingTest.Common.Models;
using ProArch.CodingTest.Common.Repository;
using ProArch.CodingTest.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProArch.CodingTest.Services
{
    public class InternalInvoiceService : IInvoiceService
    {
        private IInvoiceRepository repository;

        public InternalInvoiceService(IInvoiceRepository repository)
        {
            this.repository = repository;
        }

        public InvoiceServiceCategory ServiceType => InvoiceServiceCategory.Internal;

        public IEnumerable<SpendDetail> GetSpendDetails(int supplierId)
        {
            List<SpendDetail> result = null;
            repository.WithInvoices(invoices =>
            {
                result = invoices.Where(ivc => ivc.SupplierId == supplierId)
                 .GroupBy(ivc => ivc.InvoiceDate.Year, ivc => ivc)
                     .Select(grp => new SpendDetail()
                     {
                         Year = grp.Key,
                         TotalSpend = grp.Sum(ivc => ivc.Amount)
                     }).ToList();
            });

            return result;
        }
    }
}
