using Microsoft.EntityFrameworkCore;
using ProArch.CodingTest.Common.Repository;

namespace ProArch.CodingTest.Repository
{
    public class ProArchDbContext : DbContext, IProArchDbContext
    {
        public ProArchDbContext(DbContextOptions<ProArchDbContext> options)
            : base(options)
        {

        }
        public DbSet<SupplierData> Suppliers { get; set; }
        public DbSet<InvoiceData> Invoices { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //    string connstr = optionsBuilder.
        //        "Data Source=codingtest.database.windows.net;Initial Catalog=proarchdb;Integrated Security=false;User=proarchadmin;Password=pass@word1";// 
        //       // ConfigurationManager.ConnectionStrings[0].ConnectionString;            
        //    optionsBuilder.UseSqlServer(connstr);
        //}
    }
}
