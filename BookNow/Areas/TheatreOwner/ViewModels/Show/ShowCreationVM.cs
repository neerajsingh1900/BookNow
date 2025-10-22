using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookNow.Web.Areas.TheatreOwner.ViewModels.Show
{
    public class ShowCreationVM
    {
        [Required]
        public int ScreenId { get; set; }

        [Required(ErrorMessage = "A movie must be selected.")]
        [Display(Name = "Movie")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Start Time is required.")]
        [DataType(DataType.DateTime)]
        [FutureTime(ErrorMessage = "Show must be scheduled for at least 5 minutes in the future.")]
        [Display(Name = "Start Date and Time")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(30, 480, ErrorMessage = "Duration must be between 30 and 480 minutes.")]
        [Display(Name = "Total Show Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        public IEnumerable<SelectListItem>? MovieList { get; set; }
    }

    public class FutureTimeAttribute : ValidationAttribute
    {
        private const int FutureBufferMinutes = 5; // Allow 5 minutes for processing/buffer

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime time)
            {
                if (time < DateTime.Now.AddMinutes(FutureBufferMinutes))
                {
                    return new ValidationResult(ErrorMessage ?? $"The start time must be at least {FutureBufferMinutes} minutes in the future.");
                }
            }
             return ValidationResult.Success;
        }
    }
}
