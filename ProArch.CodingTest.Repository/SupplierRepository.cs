using ProArch.CodingTest.Common.Models;
using ProArch.CodingTest.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProArch.CodingTest.Repository
{
    public class SupplierRespository : ISupplierRepository
    {
        private ICodingTestDbContext dbContext;

        public SupplierRespository(ICodingTestDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void WithSupplierCompany(Action<Supplier> processor,int id)
        {
            using (dbContext)
            {
                processor?.Invoke(dbContext.Suppliers.FirstOrDefault(s => s.Id == id));
            }
        }
    }
}
