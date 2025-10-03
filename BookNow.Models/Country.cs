using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; } // pk, increment

        public string Name { get; set; } = null!; // varchar

        public string Code { get; set; } = null!; // varchar(3)

        // Navigation property for Cities (One-to-Many)
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
    }
}
