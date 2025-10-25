using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.ShowDTOs
{
    public class ShowMovieDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
    }
}
