using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; } // pk, increment

        public string Name { get; set; } = null!; // varchar

       
        public int CountryId { get; set; }

       
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; } = null!;

      
        public virtual ICollection<Theatre> Theatres { get; set; } = new List<Theatre>();
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
