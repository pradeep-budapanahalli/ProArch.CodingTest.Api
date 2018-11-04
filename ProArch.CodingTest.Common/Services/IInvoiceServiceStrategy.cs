using ProArch.CodingTest.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProArch.CodingTest.Common.Services
{
    public interface IInvoiceServiceStrategy
    {
        IInvoiceService GetService(InvoiceServiceType serviceType);
    }
}
