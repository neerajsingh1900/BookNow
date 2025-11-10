using AutoMapper;
using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces;
using BookNow.Application.Validation.ScreenValidations;
using BookNow.Models;
using BookNow.Utility;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ShowService> _logger;
        public ShowService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ShowService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ScreenMetadataDTO> GetScreenMetadataAsync(int screenId)
        {
            _logger.LogInformation("Fetching metadata for ScreenId: {ScreenId}", screenId);

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
            _logger.LogInformation("Fetching shows for ScreenId: {ScreenId}", screenId);

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
            _logger.LogInformation("Attempting to add new show for ScreenId: {ScreenId}, MovieId: {MovieId}", dto.ScreenId, dto.MovieId);

            var endTime = dto.StartTime.AddMinutes(dto.DurationMinutes);

            var seats = await _unitOfWork.Seat.GetSeatsByScreenAsync(dto.ScreenId);
          
            if (seats == null || !seats.Any())
            {
                _logger.LogWarning("Cannot add show; no seats found for ScreenId: {ScreenId}", dto.ScreenId);
                throw new NotFoundException($"No seats configured for Screen ID {dto.ScreenId}. Cannot create show.");
            }

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
           
            _logger.LogInformation("Successfully added new show (ShowId: {ShowId}) for ScreenId: {ScreenId}", show.ShowId, dto.ScreenId);

            return show;
        }




        public async Task<ShowMovieListDTO> GetShowCreationDataAsync(int screenId)
        {
            _logger.LogInformation("Fetching movie list for show creation (ScreenId: {ScreenId})", screenId);

            var movies = await _unitOfWork.Movie.GetAllAsync();

          
            if (movies == null || !movies.Any())
            {
                _logger.LogWarning("No movies available for scheduling at ScreenId: {ScreenId}", screenId);

                throw new ValidationException("No movies are currently available to schedule.");
            }


            var availableMovies = _mapper.Map<IEnumerable<ShowMovieDTO>>(movies.OrderBy(m => m.Title));
         
            _logger.LogInformation("Found {MovieCount} available movies for show creation (ScreenId: {ScreenId})",
               availableMovies.Count(), screenId);
          
            return new ShowMovieListDTO
            {
                ScreenId = screenId,
                AvailableMovies = availableMovies
            };
        }

    }
}
