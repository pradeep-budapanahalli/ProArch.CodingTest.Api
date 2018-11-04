using ProArch.CodingTest.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProArch.CodingTest.Repository
{
    public class InvoiceRespository : IInvoiceRespository
    {
        private IProArchDbContext dbContext;

        public InvoiceRespository(IProArchDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void WithInvoices(Action<IQueryable<InvoiceData>> processor)
        {
            using (dbContext)
            {
                processor?.Invoke(dbContext.Invoices.AsQueryable());
            }
        }
    }
}
