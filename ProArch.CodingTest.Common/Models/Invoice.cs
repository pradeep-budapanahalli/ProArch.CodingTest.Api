using System;
using System.Collections.Generic;
using System.Text;

namespace ProArch.CodingTest.Common.Models
{
    public class Invoice
    {
        public int SupplierId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public decimal Amount { get; set; }
    }
}
