using AutoMapper;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Exceptions; 
using BookNow.Application.Interfaces;
using BookNow.Models;
using BookNow.Models.Interfaces;
using BookNow.Utility;
using Microsoft.AspNetCore.Identity;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TheatreUpsertDTOValidator _validator;

        public TheatreService(IUnitOfWork unitOfWork, IMapper mapper, TheatreUpsertDTOValidator validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        // --- 1. Add Theatre ---
        public async Task<TheatreDetailDTO> AddTheatreAsync(string ownerId, TheatreUpsertDTO dto)
        {

            var validationResult = await _validator.ValidateAsync(dto);
          
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                throw new ApplicationValidationException(string.Join(" | ", errorMessages));
            }   

            var theatre = Theatre.CreateNew(
            dto.TheatreName,
            dto.Email,
            dto.PhoneNumber,
            dto.CityId,
            dto.Address,
            ownerId, 
            SD.Status_PendingApproval 
        );

            await _unitOfWork.Theatre.AddAsync(theatre);
            await _unitOfWork.SaveAsync();

          
            var fullTheatre = await _unitOfWork.Theatre.GetAsync(
                t => t.TheatreId == theatre.TheatreId,
                includeProperties: "City.Country,Screens"); 

            return _mapper.Map<TheatreDetailDTO>(fullTheatre); 
        }
    
        public async Task<TheatreDetailDTO> UpdateTheatreAsync(int theatreId, TheatreUpsertDTO dto, string ownerId)
        {   
            var theatre = await _unitOfWork.Theatre.GetAsync(t => t.TheatreId == theatreId);
            if (theatre == null)
                throw new NotFoundException($"Theatre with ID {theatreId} not found.");


            if (theatre.OwnerId != ownerId)
                throw new UnauthorizedAccessException();

            theatre.UpdateDetails(dto.TheatreName, dto.Address, dto.CityId,
                dto.PhoneNumber, dto.Email);

             _unitOfWork.Theatre.Update(theatre);
            await _unitOfWork.SaveAsync();

            var fullTheatre = await _unitOfWork.Theatre.GetAsync(
                t => t.TheatreId == theatreId,
                includeProperties: "City.Country,Screens");

            return _mapper.Map<TheatreDetailDTO>(fullTheatre);
        }
        public async Task<TheatreDetailDTO> GetTheatreByIdAsync(int theatreId, string ownerId)
        {
            var theatre = await _unitOfWork.Theatre.GetAsync(
                t => t.TheatreId == theatreId && t.OwnerId == ownerId,
                includeProperties: "City.Country,Screens"
            );

            if (theatre == null)
                throw new NotFoundException($"Theatre with ID {theatreId} not found or you are not the owner.");

            return _mapper.Map<TheatreDetailDTO>(theatre);
        }


        public async Task<IEnumerable<TheatreDetailDTO>> GetOwnerTheatresAsync(string ownerId)
        {
            var theatres = await _unitOfWork.Theatre.GetAllAsync(
               filter: t => t.OwnerId == ownerId,
               includeProperties: "City.Country,Screens");
          
              return  _mapper.Map<IEnumerable<TheatreDetailDTO>>(theatres);
           
        }

        // --- 2. Add Screen and Seats ---
        public async Task<int> AddScreenAndSeatsAsync(int theatreId, ScreenUpsertDTO dto)
        {
            // Defensive Check 1: Theatre Existence
            var theatre = await _unitOfWork.Theatre.GetAsync(t => t.TheatreId == theatreId);
            if (theatre == null)
            {
                throw new ApplicationValidationException($"Theatre with ID {theatreId} not found.");
            }

            // Defensive Check 2: Screen Number Uniqueness
            // Note: The ownerId check for security must be done in the Controller layer!
            if (!await _unitOfWork.Screen.IsScreenNumberUniqueAsync(theatreId, dto.ScreenNumber, dto.ScreenId))
            {
                throw new ValidationException($"Screen number '{dto.ScreenNumber}' already exists in this theatre.");
            }

            // 1. Create Screen Entity
            var screen = new Screen
            {
                TheatreId = theatreId,
                ScreenNumber = dto.ScreenNumber,
                TotalSeats = dto.NumberOfRows * dto.SeatsPerRow,
                DefaultSeatPrice = dto.DefaultSeatPrice
            };
            await _unitOfWork.Screen.AddAsync(screen);
            // Save now to get the ScreenId for seat generation
            await _unitOfWork.SaveAsync();

            // 2. Generate Seat Entities (Row by Row, Column by Column)
            var seatsToGenerate = new List<Seat>();
            for (int r = 1; r <= dto.NumberOfRows; r++)
            {
                // Generate Row Label (e.g., A, B, C...)
                char rowLabel = (char)('A' + r - 1);
                for (int c = 1; c <= dto.SeatsPerRow; c++)
                {
                    seatsToGenerate.Add(new Seat
                    {
                        ScreenId = screen.ScreenId,
                        RowLabel = rowLabel.ToString(),
                        SeatNumber = $"{rowLabel}{c}",
                        SeatIndex = c
                    });
                }
            }

            // 3. Add Seats
            await _unitOfWork.Seat.AddRangeAsync(seatsToGenerate);
            await _unitOfWork.SaveAsync();

            return screen.ScreenId;
        }

        public async Task<IEnumerable<Screen>> GetTheatreScreensAsync(int theatreId)
        {
            // Note: The ownerId check for security must be done in the Controller layer!
            return await _unitOfWork.Screen.GetScreensByTheatreAsync(theatreId);
        }


     //    --- 3. Add Show and Generate Seat Instances ---
        public async Task<Show> AddShowAsync(ShowCreationDTO dto)
        {
            // Calculate EndTime
            var endTime = dto.StartTime.AddMinutes(dto.DurationMinutes);

            // Defensive Check 1: Screen Existence
            var screen = await _unitOfWork.Screen.GetAsync(s => s.ScreenId == dto.ScreenId);
            if (screen == null)
            {
                throw new ApplicationValidationException($"Screen with ID {dto.ScreenId} not found.");
            }

            // Defensive Check 2: Movie Existence
            var movie = await _unitOfWork.Movie.GetAsync(m => m.MovieId == dto.MovieId);
            if (movie == null)
            {
                throw new ApplicationValidationException($"Movie with ID {dto.MovieId} not found.");
            }

            // Defensive Check 3: Time Conflict
            if (await _unitOfWork.Show.IsShowTimeConflictingAsync(dto.ScreenId, dto.StartTime, endTime))
            {
                throw new ValidationException("A show is already scheduled on this screen during the specified time.");
            }

            // 1. Create Show Entity
            var show = new Show
            {
                ScreenId = dto.ScreenId,
                MovieId = dto.MovieId,
                StartTime = dto.StartTime,
                EndTime = endTime
            };
            await _unitOfWork.Show.AddAsync(show);
            // Save now to get the ShowId
            await _unitOfWork.SaveAsync();

            // 2. Get Seats for the Screen
            var seats = await _unitOfWork.Seat.GetSeatsByScreenAsync(dto.ScreenId);

            // 3. Generate Seat Instances
            var seatInstances = new List<SeatInstance>();
            foreach (var seat in seats)
            {
                seatInstances.Add(new SeatInstance
                {
                    ShowId = show.ShowId,
                    SeatId = seat.SeatId,
                    State = SD.State_Available,
                    LastUpdated = DateTime.UtcNow
                    // RowVersion will be handled by the database
                });
            }

            // 4. Add Seat Instances
            await _unitOfWork.SeatInstance.AddRangeAsync(seatInstances);
            await _unitOfWork.SaveAsync();

            return show;
        }

        public async Task<IEnumerable<Show>> GetScreenShowsAsync(int screenId)
        {
            // This method might need to be implemented on the repository side for efficiency
            var shows = await _unitOfWork.Show.GetAllAsync(
                filter: s => s.ScreenId == screenId,
                orderBy: q => q.OrderBy(s => s.StartTime),
                includeProperties: "Movie"); // Include Movie details

            return shows;
        }
        public async Task<bool> IsOwnerOfTheatreAsync(string userId, int theatreId)
        {
            // Use AsNoTracking for an efficient read-only check
            var theatre = await _unitOfWork.Theatre.GetAsync(
                t => t.TheatreId == theatreId && t.OwnerId == userId,
                tracked: false);

            return theatre != null;
        }

    }
}