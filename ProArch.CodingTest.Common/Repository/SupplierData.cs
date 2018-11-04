using System.ComponentModel.DataAnnotations;

namespace ProArch.CodingTest.Common.Repository
{
    public class SupplierData
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsExternal { get; set; }
    }
}