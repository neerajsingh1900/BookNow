
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        public int? CityId { get; set; }

        [Required]
        [RegularExpression("Admin|Producer|Owner|User", ErrorMessage = "Invalid role.")]
 

        public string? GoogleId { get; set; }

        [Url(ErrorMessage = "Invalid profile image URL.")]
        public string? ProfileImageUrl { get; set; }

        public string? ProfileData { get; set; }    // JSON
    }
}