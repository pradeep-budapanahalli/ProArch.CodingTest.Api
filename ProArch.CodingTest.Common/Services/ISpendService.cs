using ProArch.CodingTest.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProArch.CodingTest.Common.Services
{
    public interface ISpendService
    {
        SpendSummary GetTotalSpend(int supplierId);
    }
}
