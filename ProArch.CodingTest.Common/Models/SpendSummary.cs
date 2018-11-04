using System.Collections.Generic;

namespace ProArch.CodingTest.Common.Models
{
    public class SpendSummary
    {
        public string Name { get; set; }

        public List<SpendDetail> Years { get; set; }
    }
}