using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CustomerDTOs.SearchDTOs
{
    public class MovieListingDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = null!;
        public string Language { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public int Duration { get; set; } // In minutes
        public string PosterUrl { get; set; } = null!;

        public DateOnly ReleaseDate { get; set; }
    }
}
