using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.MovieDTOs
{
    public class MovieCreateDTO
    {
        [Required] public string Title { get; set; }
        [Required] public string Genre { get; set; }
        [Required] public string Language { get; set; }
        [Range(30, 300)] public int Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ?PosterUrl { get; set; }

        public IFormFile? PosterFile { get; set; }
    }

}
