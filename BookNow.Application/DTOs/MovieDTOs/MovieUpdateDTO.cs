using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.MovieDTOs
{
    public class MovieUpdateDTO
    {
        // FIX: All strings must be nullable (string?) to allow partial updates.
        // Otherwise, validation treats them as [Required] by default.
        public string? Title { get; set; }
        public string? Genre { get; set; }
        public string? Language { get; set; }

        public int? Duration { get; set; } // int? is also necessary if omitting duration
        public DateTime? ReleaseDate { get; set; }
        public string? PosterUrl { get; set; }
    }

}
