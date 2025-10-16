using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookNow.Web.Areas.TheatreOwner.ViewModels.Show
{
    /// <summary>
    /// ViewModel for the Show scheduling form.
    /// </summary>
    public class ShowCreationVM
    {
        [Required]
        public int ScreenId { get; set; }

        [Required(ErrorMessage = "A movie must be selected.")]
        [Display(Name = "Movie")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Start Time is required.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date and Time")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(30, 480, ErrorMessage = "Duration must be between 30 and 480 minutes.")]
        [Display(Name = "Total Show Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        // UI helper property for Movie selection dropdown
        public IEnumerable<SelectListItem>? MovieList { get; set; }
    }
}
