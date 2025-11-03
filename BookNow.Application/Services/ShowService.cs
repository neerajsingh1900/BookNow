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
