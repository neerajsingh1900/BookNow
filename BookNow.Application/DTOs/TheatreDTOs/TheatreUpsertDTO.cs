//using System.ComponentModel.DataAnnotations;

//namespace BookNow.Application.DTOs.TheatreDTOs
//{
//    // DTO for creating or updating a Theatre
//    public class TheatreUpsertDTO
//    {
//        public int? TheatreId { get; set; } // Null for creation

//        [Required]
//        [MaxLength(100)]
//        public string TheatreName { get; set; } = null!;

//        [Required]
//        public int CityId { get; set; }

//        [Required]
//        [MaxLength(250)]
//        public string Address { get; set; } = null!;

//        [Required]
//        [Phone]
//        public string PhoneNumber { get; set; } = null!;

//        [Required]
//        [EmailAddress]
//        public string Email { get; set; } = null!;

//        // Status is often set by the system upon creation (e.g., "PendingApproval")
//    }


//}
// BookNow.Application/DTOs/TheatreDTOs/TheatreUpsertDTO.cs
namespace BookNow.Application.DTOs.TheatreDTOs
{
    
    public class TheatreUpsertDTO
    {
        public int? TheatreId { get; set; }
        public string TheatreName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string Address { get; set; } = string.Empty;
        // NOTE: OwnerId is passed as a separate argument to the service, 
        // as it's a security/context concern, not raw user input.
    }
}