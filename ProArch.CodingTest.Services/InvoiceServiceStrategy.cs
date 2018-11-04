using ProArch.CodingTest.Common.DTO;
using ProArch.CodingTest.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProArch.CodingTest.Services
{
    public class InvoiceServiceStrategy : IInvoiceServiceStrategy
    {
        
        private Dictionary<InvoiceServiceType, IInvoiceService> invoiceServiceStrategy;
        public InvoiceServiceStrategy(IEnumerable<IInvoiceService> invoiceServices)
        {
            this.invoiceServiceStrategy = new Dictionary<InvoiceServiceType, IInvoiceService>();
            foreach (var service in invoiceServices)
            {
                invoiceServiceStrategy.TryAdd(service.ServiceType, service);
            }
        }
        public IInvoiceService GetService(InvoiceServiceType serviceType)
        {
            if (!invoiceServiceStrategy.ContainsKey(serviceType))
            {
                throw new ApplicationException("External Invoice Service not configured.");
            }
            return invoiceServiceStrategy[serviceType];
        }
    }
}
