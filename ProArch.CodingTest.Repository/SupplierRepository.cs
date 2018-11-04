using ProArch.CodingTest.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProArch.CodingTest.Repository
{
    public class SupplierRespository : ISupplierRespository
    {
        private IProArchDbContext dbContext;

        public SupplierRespository(IProArchDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void WithSupplierCompany(Action<SupplierData> processor,int id)
        {
            using (dbContext)
            {
                processor?.Invoke(dbContext.Suppliers.FirstOrDefault(s => s.Id == id));
            }
        }
    }
}
