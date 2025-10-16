using System;

namespace BookNow.Application.DTOs.MovieDTOs
{
    public class MovieReadDTO
    {
        public int MovieId { get; set; }

        public string Title { get; set; } = "Unknown";
        public string Genre { get; set; } = "Unknown";
        public string Language { get; set; } = "Unknown";

        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }

        public string PosterUrl { get; set; } = "/images/default-poster.png";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
