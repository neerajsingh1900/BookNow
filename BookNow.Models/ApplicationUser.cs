
using BookNow.Models;
using BookNow.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookNow.Models
{
    
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; } = null!;

       
        public int? CityId { get; set; }

      
        [ForeignKey("CityId")]
        public virtual City? City { get; set; }

        
        [Required]
        [RegularExpression("Admin|Producer|Owner|User", ErrorMessage = "Invalid role.")]
        public string Role { get; set; } = "User";

        [StringLength(450)] 
        public string? GoogleId { get; set; }

        [Url(ErrorMessage = "Invalid profile image URL.")]
        public string? ProfileImageUrl { get; set; }

       
        [Column(TypeName = "nvarchar(MAX)")]
        public string? ProfileData { get; set; }
      

        public virtual ICollection<Movie> ProducedMovies { get; set; } = new List<Movie>();
        public virtual ICollection<Theatre> OwnedTheatres { get; set; } = new List<Theatre>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}