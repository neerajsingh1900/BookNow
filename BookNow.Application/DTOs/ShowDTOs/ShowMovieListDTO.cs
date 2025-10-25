using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.ShowDTOs
{
    public class ShowMovieListDTO
    {
        public int ScreenId { get; set; }

        public IEnumerable<ShowMovieDTO> AvailableMovies { get; set; } = new List<ShowMovieDTO>();
    }
}
