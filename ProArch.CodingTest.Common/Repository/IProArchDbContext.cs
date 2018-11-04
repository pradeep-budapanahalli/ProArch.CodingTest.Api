using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProArch.CodingTest.Common.Repository
{
    public interface IProArchDbContext : IDisposable
    {
        DbSet<SupplierData> Suppliers { get; set; }

        DbSet<InvoiceData> Invoices { get; set; }
    }
}
