using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProArch.CodingTest.Common.Repository
{
    public interface ISupplierRespository
    {
        void WithSupplierCompany(Action<SupplierData> processor,int id);
    }
}
