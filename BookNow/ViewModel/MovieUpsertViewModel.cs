// BookNow.Web/ViewModels/MovieUpsertViewModel.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Web.ViewModels
{
    public class MovieUpsertViewModel
    {
        public int? MovieId { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? Genre { get; set; }
        public string? Language { get; set; }
        public int Duration { get; set; }

        // Use DateOnly in your model if you do so globally
        public DateOnly ReleaseDate { get; set; }

        // File uploaded from form
        [Display(Name = "Poster")]
        public IFormFile? PosterFile { get; set; }

        // Keep existing URL to show preview or reuse when editing without new file
        //public string? PosterUrl { get; set; }
    }
}
