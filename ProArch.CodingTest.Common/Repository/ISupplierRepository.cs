﻿using ProArch.CodingTest.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProArch.CodingTest.Common.Repository
{
    public interface ISupplierRepository
    {
        void WithSupplierCompany(Action<Supplier> processor,int id);
    }
}
