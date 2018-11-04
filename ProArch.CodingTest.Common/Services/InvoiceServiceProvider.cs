using ProArch.CodingTest.Common.Models;

namespace ProArch.CodingTest.Common.Services
{
    public interface IInvoiceServiceProvider
    {
        IInvoiceService GetService(InvoiceServiceCategory serviceType);
    }
}
