using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Web.ViewModels
{
    public class CustomerMovieViewModel
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = null!;
        public string? Genre { get; set; }
        public string? Language { get; set; }
        public int Duration { get; set; }
        public string? PosterUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
