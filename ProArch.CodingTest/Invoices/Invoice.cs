﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProArch.CodingTest.Invoices
{
    public class Invoice
    {
        public int SupplierId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public decimal Amount { get; set; }
    }
}
