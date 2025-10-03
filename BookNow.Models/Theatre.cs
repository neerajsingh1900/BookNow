using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models
{
    public class Theatre
    {
        [Key]
        public int TheatreId { get; set; } // pk, increment

        // Foreign Key to Users (OwnerId varchar [ref: > Users.UserId])
        public string OwnerId { get; set; } = null!;

        // Navigation property for Owner (Many-to-One)
        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; } = null!;

        [Required]
        public string TheatreName { get; set; } = null!; // varchar

        // Foreign Key to Cities (ref: > Cities.CityId)
        public int CityId { get; set; }

        // Navigation property for City (Many-to-One)
        [ForeignKey("CityId")]
        public virtual City City { get; set; } = null!;

        public string Address { get; set; } = null!; // varchar

        [StringLength(15)]
        public string? PhoneNumber { get; set; } // varchar(15)

        [EmailAddress]
        public string Email { get; set; } = null!; // varchar [unique]

        [StringLength(20)]
        public string Status { get; set; } = "Active"; // varchar(20)

        // Navigation collections
        public virtual ICollection<Screen> Screens { get; set; } = new List<Screen>();
    }
}
