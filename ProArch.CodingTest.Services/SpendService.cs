using Polly;
using Polly.CircuitBreaker;
using ProArch.CodingTest.Common.Models;
using ProArch.CodingTest.Common.Exception;
using ProArch.CodingTest.Common.Repository;
using ProArch.CodingTest.Common.Services;
using ProArch.CodingTest.External;
using ProArch.CodingTest.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
namespace ProArch.CodingTest.Services
{
    public class SpendService : ISpendService
    {
        private ISupplierRespository supplierRepository;
        private IInvoiceServiceStrategy invoiceServiceStrategy;
        private Policy policy;
        
        public SpendService(ISupplierRespository supplerRepository, IInvoiceServiceStrategy invoiceServiceStrategy, Policy policy)
        {
            this.supplierRepository = supplerRepository;
            this.invoiceServiceStrategy = invoiceServiceStrategy;
            this.policy = policy;
        }

        public SpendSummary GetTotalSpend(int supplierId)
        {
            SpendSummary summary = null;
            supplierRepository.WithSupplierCompany(supplier =>
            {
                if (supplier == null)
                {
                    throw new ApplicationException(string.Format("Invalide Supplier Id {0}", supplierId));
                }
                summary = new SpendSummary
                {
                    Name = supplier.Name,
                    Years = GetSpendDetails(supplier)
                };
            }, supplierId);

            return summary;
        }
    

        private List<SpendDetail> GetSpendDetails(SupplierData supplier)
        {
            var serviceType = supplier.IsExternal ? InvoiceServiceType.External : InvoiceServiceType.Internal;
            try
            {
                List<SpendDetail> summary = null;
                policy.Execute(() =>
                {
                    summary = invoiceServiceStrategy.GetService(serviceType)
                        .GetSpendDetails(supplier.Id)?.ToList();
                });
                return summary;
            }
            catch (Exception ex)
            {
                if (ex is BrokenCircuitException ||
                ex is ExternalInvoiceServiceException)
                {
                    return invoiceServiceStrategy.GetService(InvoiceServiceType.Failover)
                        .GetSpendDetails(supplier.Id)
                        .ToList();
                }
                throw ex;
            }
        }
    }
}
