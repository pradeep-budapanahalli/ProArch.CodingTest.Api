using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace ProArch.CodingTest.Common.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        public int SupplierId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public decimal Amount { get; set; }
    }
}
