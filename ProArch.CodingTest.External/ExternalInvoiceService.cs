using System;
using System.Collections.Generic;

namespace ProArch.CodingTest.External
{
    // Note: this is an external SDK we have no control over
    // nothing can be changed here

    public static class ExternalInvoiceService
    {
        public static bool IsServiceDown;

        public static Dictionary<string, ExternalInvoice[]> Invoices;
        static ExternalInvoiceService()
        {
            // initial seed data
            Invoices = new Dictionary<string, ExternalInvoice[]>()
            {
                {"2",new []{ new ExternalInvoice() { Year = 2018, TotalAmount=15000},
                 new ExternalInvoice(){ Year = 2017, TotalAmount = 10000 } ,
                 new ExternalInvoice() { Year = 2016, TotalAmount=5000} } }
            };
        }
        public static ExternalInvoice[] GetInvoices(string supplierId)
        {
            if (IsServiceDown)
            {
                throw new ApplicationException();
            }
            return Invoices != null && Invoices.ContainsKey(supplierId) ? Invoices[supplierId] : new ExternalInvoice[0];
        }
    }
}