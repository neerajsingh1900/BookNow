using AutoMapper;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ScreenDTOs.BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces;
using BookNow.Application.Validation.ScreenValidations;
using BookNow.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    
    public class ScreenService : IScreenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ScreenService> _logger;

        public ScreenService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScreenService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        
        public async Task<ScreenDetailsDTO?> GetScreenDetailsByIdAsync(int screenId)
        {
            _logger.LogInformation("Fetching screen details for ScreenId: {ScreenId}", screenId);
            var screen = await _unitOfWork.Screen.GetAsync(s => s.ScreenId == screenId, 
           includeProperties: "Seats,Shows",
                tracked: false
            );

            if (screen == null) return null;
            _logger.LogInformation("Successfully fetched screen details for ScreenId: {ScreenId}", screenId);
            return _mapper.Map<ScreenDetailsDTO>(screen);
        }

        
        public async Task<IEnumerable<ScreenDetailsDTO>> GetScreensByTheatreIdAsync(int theatreId)
        {
            _logger.LogInformation("Fetching screens for TheatreId: {TheatreId}", theatreId);

            if (theatreId <= 0)
            {
                _logger.LogWarning("Invalid TheatreId provided: {TheatreId}", theatreId);

                throw new ApplicationValidationException("Invalid Theatre ID provided for fetching screens.");
            }

           var screens = await _unitOfWork.Screen.GetAllAsync(
               filter: s => s.TheatreId == theatreId,
               orderBy: q => q.OrderBy(s => s.ScreenNumber),
               includeProperties: "Shows");
            var screenDtos = _mapper.Map<IEnumerable<ScreenDetailsDTO>>(screens).ToList();

            
            foreach (var dto in screenDtos)
            {
                dto.HasActiveBookings = await _unitOfWork.Screen.HasActiveBookings(dto.ScreenId);
            }

            _logger.LogInformation("Fetched {Count} screens for TheatreId: {TheatreId}", screenDtos.Count(), theatreId);

            return screenDtos;
        }

        public async Task<ScreenDetailsDTO> CreateScreenAsync(ScreenUpsertDTO dto)
        {
            _logger.LogInformation("Creating new screen for TheatreId: {TheatreId}", dto.TheatreId);

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var theatre = await _unitOfWork.Theatre.GetAsync(t => t.TheatreId == dto.TheatreId, tracked: false);
                if (theatre == null)
                {
                    _logger.LogError("Theatre not found with ID: {TheatreId}", dto.TheatreId);

                    throw new NotFoundException($"Theatre with ID {dto.TheatreId} not found.");
                }


                Screen screen = _mapper.Map<Screen>(dto);
                screen.TotalSeats = dto.NumberOfRows * dto.SeatsPerRow;
                await _unitOfWork.Screen.AddAsync(screen);


                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Generating {Rows}x{Cols} seats for ScreenId: {ScreenId}",
                    dto.NumberOfRows, dto.SeatsPerRow, screen.ScreenId);

                var seatsToGenerate = new List<Seat>();
                    for (int r = 1; r <= dto.NumberOfRows; r++)
                    {
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

               
                await _unitOfWork.Seat.BulkInsertAsync(seatsToGenerate);
                await _unitOfWork.SaveAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Screen created successfully with ScreenId: {ScreenId}", screen.ScreenId);


                return _mapper.Map<ScreenDetailsDTO>(screen);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while creating screen for TheatreId: {TheatreId}", dto.TheatreId);

                throw;
            }
        }
       
        public async Task SoftDeleteScreenAsync(int screenId)
        {
          
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Screen.CascadeSoftDelete(screenId);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("Screen {ScreenId} successfully soft-deleted.", screenId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Transaction rollback during soft delete for Screen {ScreenId}", screenId);
                throw;
            }
        }

        public async Task UpdateScreenAsync(ScreenUpsertDTO dto)
    {
            _logger.LogInformation("Updating ScreenId: {ScreenId}", dto.ScreenId);

            if (!dto.ScreenId.HasValue || dto.ScreenId.Value <= 0)
        {
                _logger.LogWarning("Invalid ScreenId for update: {ScreenId}", dto.ScreenId);

                throw new ApplicationValidationException("ScreenId is required for an update operation.");
        }
       
         var theatre = await _unitOfWork.Theatre.GetAsync(t => t.TheatreId == dto.TheatreId, tracked: false);
        if (theatre == null)
        {
                _logger.LogError("Theatre not found with ID: {TheatreId}", dto.TheatreId);

                throw new NotFoundException($"Theatre with ID {dto.TheatreId} not found.");
        }


    
      Screen screen = await _unitOfWork.Screen.GetAsync(s => s.ScreenId == dto.ScreenId.Value);
                

            if (screen == null)
            throw new NotFoundException($"Screen with ID {dto.ScreenId.Value} not found.");
        
            var existingSeats = await _unitOfWork.Seat.GetSeatsByScreenAsync(screen.ScreenId);
            int existingRows = existingSeats.Select(s => s.RowLabel).Distinct().Count();
            int existingSeatsPerRow = existingSeats.Any()
                ? existingSeats.GroupBy(s => s.RowLabel).Select(g => g.Count()).Max()
                : 0;

            bool layoutChanged = existingRows != dto.NumberOfRows || existingSeatsPerRow != dto.SeatsPerRow;

          

       screen.ScreenNumber = dto.ScreenNumber;
        screen.DefaultSeatPrice = dto.DefaultSeatPrice;
        screen.TotalSeats = dto.NumberOfRows * dto.SeatsPerRow; 

        _unitOfWork.Screen.Update(screen);


      
        if (layoutChanged)
        {
                _logger.LogInformation("Seat layout changed for ScreenId: {ScreenId}. Regenerating seats.", screen.ScreenId);

                if (existingSeats.Any())
                _unitOfWork.Seat.RemoveRange(existingSeats); 
               

            var seatsToGenerate = new List<Seat>();
            for (int r = 1; r <= dto.NumberOfRows; r++)
            {
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

            await _unitOfWork.Seat.AddRangeAsync(seatsToGenerate);
        }

            await _unitOfWork.SaveAsync();
       _logger.LogInformation("Screen updated successfully with ScreenId: {ScreenId}", screen.ScreenId);

        }
    
    }
}


