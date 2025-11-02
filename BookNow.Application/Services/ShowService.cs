using AutoMapper;
using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Application.Validation.ScreenValidations;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using BookNow.Utility;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    public class ShowService:IShowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShowService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ScreenMetadataDTO> GetScreenMetadataAsync(int screenId)
        {
            var screen = await _unitOfWork.Screen.GetAsync(
                s => s.ScreenId == screenId,
                includeProperties: "Theatre"
            );

            if (screen == null)
                throw new NotFoundException($"Screen ID {screenId} not found or access denied.");

            return _mapper.Map<ScreenMetadataDTO>(screen);
        }

        public async Task<IEnumerable<ShowDetailsDTO>> GetShowsForScreenAsync(int screenId)
        {
            var screenWithShows = await _unitOfWork.Screen.GetAsync(
                s => s.ScreenId == screenId,
                includeProperties: "Shows.Movie"
            );

            if (screenWithShows == null)
                throw new NotFoundException($"Screen ID {screenId} not found or access denied.");

            return _mapper.Map<IEnumerable<ShowDetailsDTO>>(screenWithShows.Shows);
        }


        public async Task<Show> AddShowAsync(ShowCreationDTO dto)
        {
            var endTime = dto.StartTime.AddMinutes(dto.DurationMinutes);

            
            var screen = await _unitOfWork.Screen.GetAsync(s => s.ScreenId == dto.ScreenId);
            
            if (screen == null)
                throw new ApplicationValidationException($"Screen with ID {dto.ScreenId} not found.");
            
           
            var movie = await _unitOfWork.Movie.GetAsync(m => m.MovieId == dto.MovieId);
            if (movie == null)
                throw new ApplicationValidationException($"Movie with ID {dto.MovieId} not found.");

           
            if (dto.StartTime < DateTime.Now.AddMinutes(30))
                throw new ValidationException("Show start time cannot be in the past and must be at least 30 minutes in the future.");

           
            const int CLEANUP_BUFFER_MINUTES = 15;
            if (dto.DurationMinutes < (movie.Duration + CLEANUP_BUFFER_MINUTES))
                throw new ValidationException($"Total show duration ({dto.DurationMinutes} mins) cannot be less than the movie's run-time ({movie.Duration} mins) plus a {CLEANUP_BUFFER_MINUTES}-minute cleanup interval.");

         
            if (await _unitOfWork.Show.IsShowTimeConflictingAsync(dto.ScreenId, dto.StartTime, endTime))
                throw new ValidationException("A show is already scheduled on this screen during the specified time.");

         
            var seats = await _unitOfWork.Seat.GetSeatsByScreenAsync(dto.ScreenId);

           
            var show = new Show
            {
                ScreenId = dto.ScreenId,
                MovieId = dto.MovieId,
                StartTime = dto.StartTime,
                EndTime = endTime,
                SeatInstances = seats.Select(seat => new SeatInstance
                {
                    SeatId = seat.SeatId,
                    State = SD.State_Available,
                    LastUpdated = DateTime.UtcNow
                }).ToList()
            };

         
            await _unitOfWork.Show.AddAsync(show);
            await _unitOfWork.SaveAsync();

            return show;
        }




        public async Task<ShowMovieListDTO> GetShowCreationDataAsync(int screenId)
        {
           
            var movies = await _unitOfWork.Movie.GetAllAsync();

          
            if (movies == null || !movies.Any())
            {
                throw new ValidationException("No movies are currently available to schedule.");
            }


            var availableMovies = _mapper.Map<IEnumerable<ShowMovieDTO>>(movies.OrderBy(m => m.Title));

            return new ShowMovieListDTO
            {
                ScreenId = screenId,
                AvailableMovies = availableMovies
            };
        }

    }
}
