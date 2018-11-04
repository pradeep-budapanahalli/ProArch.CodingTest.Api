using System.ComponentModel.DataAnnotations;

namespace ProArch.CodingTest.Common.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsExternal { get; set; }
    }
}