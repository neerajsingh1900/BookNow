
using BookNow.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookNow.Models
{
    //public class ApplicationUser : IdentityUser
    //{
    //    [Required]
    //    public string Name { get; set; }

    //    public int? CityId { get; set; }

    //    [Required]
    //    [RegularExpression("Admin|Producer|Owner|User", ErrorMessage = "Invalid role.")]
 

    //    public string? GoogleId { get; set; }

    //    [Url(ErrorMessage = "Invalid profile image URL.")]
    //    public string? ProfileImageUrl { get; set; }

    //    public string? ProfileData { get; set; }    // JSON
    //}


    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; } = null!;

        // The DDL showed UserId (PK) as int, but since you extend IdentityUser, 
        // the base PK 'Id' is used and is a string. FKs must point to this string PK.

        // CityId is the Foreign Key
        public int? CityId { get; set; }

        // Navigation property for the City (ref: > Cities.CityId)
        [ForeignKey("CityId")]
        public virtual City? City { get; set; }

        // The Role property definition was missing in your snippet
        [Required]
        [RegularExpression("Admin|Producer|Owner|User", ErrorMessage = "Invalid role.")]
        public string Role { get; set; } = "User";

        [StringLength(450)] // Ensure this is sized appropriately for unique indexing
        public string? GoogleId { get; set; }

        [Url(ErrorMessage = "Invalid profile image URL.")]
        public string? ProfileImageUrl { get; set; }

        // Store ProfileData as a string/JSON. EF Core can handle JSON serialization if needed.
        [Column(TypeName = "nvarchar(MAX)")]
        public string? ProfileData { get; set; }

        // Navigation collections for relations where ApplicationUser is the 'One' side
        public virtual ICollection<Movie> ProducedMovies { get; set; } = new List<Movie>();
        public virtual ICollection<Theatre> OwnedTheatres { get; set; } = new List<Theatre>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}