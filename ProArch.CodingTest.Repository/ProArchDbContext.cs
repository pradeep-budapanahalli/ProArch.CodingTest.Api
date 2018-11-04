using Microsoft.EntityFrameworkCore;
using ProArch.CodingTest.Common.Models;
using ProArch.CodingTest.Common.Repository;

namespace ProArch.CodingTest.Repository
{
    public class CodingTestDbContext : DbContext, ICodingTestDbContext
    {
        public CodingTestDbContext(DbContextOptions<CodingTestDbContext> options)
            : base(options)
        {

        }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }      
    }
}
