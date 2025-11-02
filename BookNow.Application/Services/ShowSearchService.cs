using AutoMapper;
using BookNow.Application.DTOs.CustomerDTOs.SearchDTOs;
using BookNow.Application.Interfaces;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    public class ShowSearchService : IShowSearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShowSearchService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<MovieListingDTO>> GetMoviesByCityAsync(int? cityId)
        {
            var movies = await _unitOfWork.Show.GetMoviesByCityAsync(cityId);

            return _mapper.Map<IEnumerable<MovieListingDTO>>(movies);
           
        }

        private async Task<IEnumerable<Show>> GetRawShowsInWindow(int movieId, int cityId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var endDate = today.AddDays(6);

            return await _unitOfWork.Show.GetShowsForMovieAndCityAsync(
                movieId, cityId, today, endDate);
        }

        private List<TheatreShowtimeDTO> GroupAndMapShows(IEnumerable<Show> rawShows)
        {
            return rawShows
               .GroupBy(s => s.Screen.Theatre)
               .Select(g => new TheatreShowtimeDTO
               {
                   TheatreId = g.Key.TheatreId,
                   TheatreName = g.Key.TheatreName,
                   Address = g.Key.Address,
                   Showtimes = _mapper.Map<List<ShowtimeDTO>>(g.OrderBy(s => s.StartTime).ToList())
               })
               .ToList();
        }

        public async Task<SelectTheatrePageDTO> GetShowtimesForWindowAsync(int movieId, int cityId)
        {

            var today = DateOnly.FromDateTime(DateTime.Today);

            var dateWindow = Enumerable.Range(0, 7).Select(i => today.AddDays(i)).ToList();

            var pageData = new SelectTheatrePageDTO { FixedDateWindow = dateWindow };

            var rawShows = await GetRawShowsInWindow(movieId, cityId);

            var movieEntity = await _unitOfWork.Movie.GetAsync(m => m.MovieId == movieId);
            if (movieEntity == null) return pageData;

            pageData.Movie = _mapper.Map<MovieListingDTO>(movieEntity);

          
            if (rawShows.Any())
            {
                pageData.Theatres = GroupAndMapShows(rawShows);

             
                pageData.AvailableDates = pageData.Theatres
                    .SelectMany(t => t.Showtimes)
                    .Select(s => DateOnly.FromDateTime(s.StartTime.Date))
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

               
                pageData.ActiveDate = pageData.AvailableDates.First();
            }

            return pageData;
        }

        public async Task<List<TheatreShowtimeDTO>> GetFilteredShowtimesForDateAsync(int movieId, int cityId, DateOnly targetDate)
        {
           
            var rawShows = await _unitOfWork.Show.GetShowsForMovieAndCityAsync(
                movieId, cityId, targetDate, targetDate); 

        
            var filteredTheatres = GroupAndMapShows(rawShows);

         
            return filteredTheatres.Where(t => t.Showtimes.Any()).ToList();
        }
    }
}
