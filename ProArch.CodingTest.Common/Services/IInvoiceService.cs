using ProArch.CodingTest.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProArch.CodingTest.Common.Services
{
    public interface IInvoiceService
    {
        InvoiceServiceType ServiceType { get; }
        IEnumerable<SpendDetail> GetSpendDetails(int supplierId);
    }
}
