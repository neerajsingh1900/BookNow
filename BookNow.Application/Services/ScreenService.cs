using AutoMapper;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ScreenDTOs.BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.Exceptions;
using BookNow.Application.Interfaces;
using BookNow.Application.Validation.ScreenValidations;
using BookNow.Models;
using BookNow.Application.RepoInterfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity; // Not strictly needed here, but kept for context consistency
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
    

        public ScreenService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
           
        }

        
        public async Task<ScreenDetailsDTO?> GetScreenDetailsByIdAsync(int screenId)
        {
          var screen = await _unitOfWork.Screen.GetAsync(s => s.ScreenId == screenId, 
           includeProperties: "Seats,Shows",
                tracked: false
            );

            if (screen == null) return null;

            return _mapper.Map<ScreenDetailsDTO>(screen);
        }

        
        public async Task<IEnumerable<ScreenDetailsDTO>> GetScreensByTheatreIdAsync(int theatreId)
        {
            if (theatreId <= 0)
            {
                throw new ApplicationValidationException("Invalid Theatre ID provided for fetching screens.");
            }

           var screens = await _unitOfWork.Screen.GetAllAsync(
               filter: s => s.TheatreId == theatreId,
               orderBy: q => q.OrderBy(s => s.ScreenNumber),
               includeProperties: "Shows"); 

            return _mapper.Map<IEnumerable<ScreenDetailsDTO>>(screens);
        }

        public async Task<ScreenDetailsDTO> CreateScreenAsync(ScreenUpsertDTO dto)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var theatre = await _unitOfWork.Theatre.GetAsync(t => t.TheatreId == dto.TheatreId, tracked: false);
                if (theatre == null)
                {
                    throw new NotFoundException($"Theatre with ID {dto.TheatreId} not found.");
                }


                Screen screen = _mapper.Map<Screen>(dto);
                screen.TotalSeats = dto.NumberOfRows * dto.SeatsPerRow;
                await _unitOfWork.Screen.AddAsync(screen);


                await _unitOfWork.SaveAsync();

               
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
                    await _unitOfWork.SaveAsync();

                await transaction.CommitAsync();

                return _mapper.Map<ScreenDetailsDTO>(screen);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    public async Task UpdateScreenAsync(ScreenUpsertDTO dto)
    {
        
        if (!dto.ScreenId.HasValue || dto.ScreenId.Value <= 0)
        {
            throw new ApplicationValidationException("ScreenId is required for an update operation.");
        }
       
         var theatre = await _unitOfWork.Theatre.GetAsync(t => t.TheatreId == dto.TheatreId, tracked: false);
        if (theatre == null)
        {
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
    }
  }
}


