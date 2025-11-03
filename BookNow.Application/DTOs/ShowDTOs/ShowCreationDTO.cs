using System;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Application.DTOs.ShowDTOs
{
   
    public class ShowCreationDTO
    {
        [Required(ErrorMessage = "Screen ID is required.")]
        public int ScreenId { get; set; }

        [Required(ErrorMessage = "Movie ID is required.")]
        public int MovieId { get; set; }


        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        [Range(60, 600, ErrorMessage = "Duration must be between 60 and 600 minutes.")]
        public int DurationMinutes { get; set; } // Movie duration + buffer time
    }
}   