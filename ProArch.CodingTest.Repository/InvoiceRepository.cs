using ProArch.CodingTest.Common.Models;
using ProArch.CodingTest.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProArch.CodingTest.Repository
{
    public class InvoiceRespository : IInvoiceRepository
    {
        private ICodingTestDbContext dbContext;

        public InvoiceRespository(ICodingTestDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void WithInvoices(Action<IQueryable<Invoice>> processor)
        {
            using (dbContext)
            {
                processor?.Invoke(dbContext.Invoices.AsQueryable());
            }
        }
    }
}
