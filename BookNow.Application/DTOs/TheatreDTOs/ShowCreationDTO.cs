using System;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Application.DTOs.TheatreDTOs
{
    // DTO for creating a Show
    public class ShowCreationDTO
    {
        [Required]
        public int ScreenId { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public int DurationMinutes { get; set; } // Movie duration + buffer time
    }
}