using System.ComponentModel.DataAnnotations;

namespace BookNow.Application.DTOs.ScreenDTOs
{
    // DTO for creating or updating a Screen
    //public class ScreenUpsertDTO
    //{
    //    public int? ScreenId { get; set; } // Null for creation

    //    [Required]
    //    public int TheatreId { get; set; }

    //    [Required]
    //    [MaxLength(50)]
    //    public string ScreenNumber { get; set; } = null!;

    //    // Used to generate seats. Not stored in TotalSeats field directly.
    //    [Required]
    //    [Range(1, 50, ErrorMessage = "Number of rows must be between 1 and 50.")]
    //    public int NumberOfRows { get; set; }

    //    [Required]
    //    [Range(1, 100, ErrorMessage = "Number of columns must be between 1 and 100.")]
    //    public int SeatsPerRow { get; set; }

    //    [Required]
    //    [Range(0.01, 99999.99)]
    //    public decimal DefaultSeatPrice { get; set; }
    //}
    public class ScreenUpsertDTO
    {
        public int? ScreenId { get; set; } // Null for creation
        public int TheatreId { get; set; }
        public string ScreenNumber { get; set; } = null!;
        public int NumberOfRows { get; set; }
        public int SeatsPerRow { get; set; }
        public decimal DefaultSeatPrice { get; set; }
    }
}   