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

        // Foreign Key to Countries (ref: > Countries.CountryId)
        public int CountryId { get; set; }

        // Navigation property for Country (Many-to-One)
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; } = null!;

        // Navigation collections for Theatres and Users (One-to-Many)
        public virtual ICollection<Theatre> Theatres { get; set; } = new List<Theatre>();
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
