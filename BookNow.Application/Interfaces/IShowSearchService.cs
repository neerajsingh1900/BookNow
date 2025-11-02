using BookNow.Application.DTOs.CustomerDTOs.SearchDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface IShowSearchService
    {
        Task<IEnumerable<MovieListingDTO>> GetMoviesByCityAsync(int? cityId);
        Task<SelectTheatrePageDTO> GetShowtimesForWindowAsync(int movieId, int cityId);

        Task<List<TheatreShowtimeDTO>> GetFilteredShowtimesForDateAsync(int movieId, int cityId, DateOnly targetDate);

    }
}
