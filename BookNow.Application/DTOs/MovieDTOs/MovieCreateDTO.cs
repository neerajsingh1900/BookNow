using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Application.DTOs.MovieDTOs
{
    public class MovieCreateDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Genre is required.")]
        [StringLength(100, ErrorMessage = "Genre cannot exceed 100 characters.")]
        public string Genre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language is required.")]
        [StringLength(100, ErrorMessage = "Language cannot exceed 100 characters.")]
        public string Language { get; set; } = string.Empty;

        [Range(30, 300, ErrorMessage = "Duration must be between 30 and 300 minutes.")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Release date is required.")]
        public DateTime? ReleaseDate { get; set; }

       
        [StringLength(500)]
        public string? PosterUrl { get; set; }

        public IFormFile? PosterFile { get; set; }
    }
}
