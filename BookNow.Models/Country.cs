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

        public string Name { get; set; } = null!; 

        public string Code { get; set; } = null!;

      
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
    }
}
