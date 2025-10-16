using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Application.DTOs.MovieDTOs
{
    public class MovieUpdateDTO
    {
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string? Title { get; set; }

        [StringLength(100, ErrorMessage = "Genre cannot exceed 100 characters.")]
        public string? Genre { get; set; }

        [StringLength(100, ErrorMessage = "Language cannot exceed 100 characters.")]
        public string? Language { get; set; }

        [Range(30, 300, ErrorMessage = "Duration must be between 30 and 300 minutes.")]
        public int? Duration { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [StringLength(500)]
        public string? PosterUrl { get; set; }

        public IFormFile? PosterFile { get; set; }
    }
}
