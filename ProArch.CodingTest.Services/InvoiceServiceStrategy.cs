using ProArch.CodingTest.Common.Models;
using ProArch.CodingTest.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProArch.CodingTest.Services
{
    public class InvoiceServiceProvider : IInvoiceServiceProvider
    {        
        private Dictionary<InvoiceServiceCategory, IInvoiceService> invoiceServiceProviders;
        public InvoiceServiceProvider(IEnumerable<IInvoiceService> invoiceServices)
        {
            this.invoiceServiceProviders = new Dictionary<InvoiceServiceCategory, IInvoiceService>();
            foreach (var service in invoiceServices)
            {
                invoiceServiceProviders.TryAdd(service.ServiceType, service);
            }
        }
        public IInvoiceService GetService(InvoiceServiceCategory serviceType)
        {
            if (!invoiceServiceProviders.ContainsKey(serviceType))
            {
                throw new ApplicationException("External Invoice Service not configured.");
            }
            return invoiceServiceProviders[serviceType];
        }
    }
}
