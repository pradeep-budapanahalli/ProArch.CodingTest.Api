using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProArch.CodingTest.Common.Repository
{
    public interface IInvoiceRespository
    {
        void WithInvoices(Action<IQueryable<InvoiceData>> processor);
    }
}
