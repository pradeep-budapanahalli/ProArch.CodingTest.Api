using Microsoft.EntityFrameworkCore;
using ProArch.CodingTest.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProArch.CodingTest.Common.Repository
{
    public interface ICodingTestDbContext : IDisposable
    {
        DbSet<Supplier> Suppliers { get; set; }

        DbSet<Invoice> Invoices { get; set; }
    }
}
