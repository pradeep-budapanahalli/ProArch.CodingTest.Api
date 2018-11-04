using ProArch.CodingTest.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProArch.CodingTest.Common.Repository
{
    public interface IInvoiceRepository
    {
        void WithInvoices(Action<IQueryable<Invoice>> processor);
    }
}
