using System.ComponentModel.DataAnnotations;

namespace BookNow.Application.DTOs.TheatreDTOs
{
    public class TheatreUpsertDTO
    {
        public int? TheatreId { get; set; }

        [Required(ErrorMessage = "Theatre name is required.")]
        [StringLength(100)]
        [RegularExpression(@"^[A-Za-z0-9\s&\-'.]+$", ErrorMessage = "Theatre name can only contain letters, numbers, spaces, apostrophes, hyphens, and ampersands.")]
        public string TheatreName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;

      
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?[0-9\s\-]{7,15}$", ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;
    }
}