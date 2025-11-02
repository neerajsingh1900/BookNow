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
        private readonly ScreenUpsertValidator _validator; 

        public ScreenService(IUnitOfWork unitOfWork, IMapper mapper, ScreenUpsertValidator validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
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
        // --- 1. Validation using Fluent Validation ---
         var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
            throw new ApplicationValidationException(string.Join(" | ", errorMessages));
        }

        // --- 2. Existence and Uniqueness Checks ---

        // Check Theatre Existence (Security: The calling API/MVC Controller must handle owner validation)
        var theatre = await _unitOfWork.Theatre.GetAsync(t => t.TheatreId == dto.TheatreId, tracked: false);
        if (theatre == null)
        {
            throw new NotFoundException($"Theatre with ID {dto.TheatreId} not found.");
        }

        // Check Screen Number Uniqueness within the theatre (ScreenId is null for creation)
        bool isUnique = await _unitOfWork.Screen.IsScreenNumberUniqueAsync(
            dto.TheatreId, dto.ScreenNumber, null); 

        if (!isUnique)
        {
            
            throw new ApplicationValidationException($"Screen number '{dto.ScreenNumber}' already exists in this theatre.");
        }

        // --- CREATE Logic ---
        Screen screen = _mapper.Map<Screen>(dto);
        screen.TotalSeats = dto.NumberOfRows * dto.SeatsPerRow;
        await _unitOfWork.Screen.AddAsync(screen);

        // Save screen creation to get the ScreenId for seat generation
        await _unitOfWork.SaveAsync();

        // Set layout changed to true to ensure seats are created
        bool layoutChanged = true;


        // --- 3. Seat Generation ---
        if (layoutChanged)
        {
            // Generate and Add New Seats
            var seatsToGenerate = new List<Seat>();
            for (int r = 1; r <= dto.NumberOfRows; r++)
            {
                char rowLabel = (char)('A' + r - 1); // A, B, C...
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
        }

     
        return _mapper.Map<ScreenDetailsDTO>(screen);
    }

    // -----------------------------------------------------------------------------

    // --- Service 2: UpdateScreenAsync ---

    public async Task UpdateScreenAsync(ScreenUpsertDTO dto)
    {
        
        if (!dto.ScreenId.HasValue || dto.ScreenId.Value <= 0)
        {
            throw new ApplicationValidationException("ScreenId is required for an update operation.");
        }

      
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
            throw new ApplicationValidationException(string.Join(" | ", errorMessages));
        }

       
         var theatre = await _unitOfWork.Theatre.GetAsync(t => t.TheatreId == dto.TheatreId, tracked: false);
        if (theatre == null)
        {
            throw new NotFoundException($"Theatre with ID {dto.TheatreId} not found.");
        }


       bool isUnique = await _unitOfWork.Screen.IsScreenNumberUniqueAsync(
            dto.TheatreId, dto.ScreenNumber, dto.ScreenId);

        if (!isUnique)
        {
            throw new ApplicationValidationException($"Screen number '{dto.ScreenNumber}' already exists in this theatre.");
            }

            Screen screen = await _unitOfWork.Screen.GetAsync(s => s.ScreenId == dto.ScreenId.Value);


            if (screen == null)
        {
            throw new NotFoundException($"Screen with ID {dto.ScreenId.Value} not found.");
        }
            var existingSeats = await _unitOfWork.Seat.GetSeatsByScreenAsync(screen.ScreenId);
            int existingRows = existingSeats.Select(s => s.RowLabel).Distinct().Count();
            int existingSeatsPerRow = existingSeats.Any()
                ? existingSeats.GroupBy(s => s.RowLabel).Select(g => g.Count()).Max()
                : 0;

            bool layoutChanged = existingRows != dto.NumberOfRows || existingSeatsPerRow != dto.SeatsPerRow;

          

       screen.ScreenNumber = dto.ScreenNumber;
        screen.DefaultSeatPrice = dto.DefaultSeatPrice;
        screen.TotalSeats = dto.NumberOfRows * dto.SeatsPerRow; // Always update total seats

        _unitOfWork.Screen.Update(screen);


        // --- 3. Seat Generation/Regeneration ---
        if (layoutChanged)
        {
           if (existingSeats.Any())
                _unitOfWork.Seat.RemoveRange(existingSeats); 
               

            // Generate and Add New Seats
            var seatsToGenerate = new List<Seat>();
            for (int r = 1; r <= dto.NumberOfRows; r++)
            {
                char rowLabel = (char)('A' + r - 1); // A, B, C...
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



    //public async Task<ScreenDetailsDTO> UpsertScreenAsync(ScreenUpsertDTO dto)
    //{
    //    // --- 1. Validation using Fluent Validation ---
    //    var validationResult = await _validator.ValidateAsync(dto);
    //    if (!validationResult.IsValid)
    //    {
    //        var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
    //        throw new ApplicationValidationException(string.Join(" | ", errorMessages));
    //    }

    //    // --- 2. Existence and Uniqueness Checks ---

    //    // Check Theatre Existence (Security: The calling API/MVC Controller must handle owner validation)
    //    var theatre = await _unitOfWork.Theatre.GetAsync(t => t.TheatreId == dto.TheatreId, tracked: false);
    //    if (theatre == null)
    //    {
    //        throw new NotFoundException($"Theatre with ID {dto.TheatreId} not found.");
    //    }

    //    // Check Screen Number Uniqueness within the theatre (excluding current screen if updating)
    //    bool isUnique = await _unitOfWork.Screen.IsScreenNumberUniqueAsync(
    //        dto.TheatreId, dto.ScreenNumber, dto.ScreenId);

    //    if (!isUnique)
    //    {
    //        // Throw ValidationException if not unique
    //        throw new ApplicationValidationException($"Screen number '{dto.ScreenNumber}' already exists in this theatre.");
    //    }

    //    Screen screen;
    //    bool layoutChanged = false;

    //    if (dto.ScreenId.HasValue && dto.ScreenId.Value > 0)
    //    {
    //        // --- UPDATE Logic ---
    //        screen = await _unitOfWork.Screen.GetAsync(s => s.ScreenId == dto.ScreenId.Value);

    //        if (screen == null)
    //        {
    //            throw new NotFoundException($"Screen with ID {dto.ScreenId.Value} not found.");
    //        }

    //        // Check if layout parameters have changed (requiring seat regeneration)
    //        // NOTE: We assume the Screen entity stores these configuration values. 
    //        // If not, this check must rely on fetching existing seat structure.
    //        if (screen.TotalSeats != (dto.NumberOfRows * dto.SeatsPerRow))
    //        {
    //            layoutChanged = true;
    //        }

    //        // Update Screen properties
    //        screen.ScreenNumber = dto.ScreenNumber;
    //        screen.DefaultSeatPrice = dto.DefaultSeatPrice;
    //        screen.TotalSeats = dto.NumberOfRows * dto.SeatsPerRow; // Always update total seats

    //        _unitOfWork.Screen.Update(screen);
    //    }



    //    else
    //    {
    //        // --- CREATE Logic ---
    //        screen = _mapper.Map<Screen>(dto);
    //        screen.TotalSeats = dto.NumberOfRows * dto.SeatsPerRow;
    //        await _unitOfWork.Screen.AddAsync(screen);

    //        // Set layout changed to true to ensure seats are created
    //        layoutChanged = true;
    //    }

    //    // Save screen changes now (needed for Create to get ScreenId, or Update before seat deletion)
    //    await _unitOfWork.SaveAsync();

    //    // --- 3. Seat Generation/Regeneration ---
    //    if (layoutChanged)
    //    {
    //        // If this is an update and the layout changed, delete all existing seats
    //        if (dto.ScreenId.HasValue)
    //        {
    //            var existingSeats = await _unitOfWork.Seat.GetSeatsByScreenAsync(screen.ScreenId);
    //            if (existingSeats.Any())
    //            {
    //                // Assume a bulk delete method exists on the repository
    //           //     _unitOfWork.Seat.DeleteRange(existingSeats);
    //                await _unitOfWork.SaveAsync(); // Save delete operation
    //            }
    //        }

    //        // Generate and Add New Seats
    //        var seatsToGenerate = new List<Seat>();
    //        for (int r = 1; r <= dto.NumberOfRows; r++)
    //        {
    //            char rowLabel = (char)('A' + r - 1); // A, B, C...
    //            for (int c = 1; c <= dto.SeatsPerRow; c++)
    //            {
    //                seatsToGenerate.Add(new Seat
    //                {
    //                    ScreenId = screen.ScreenId,
    //                    RowLabel = rowLabel.ToString(),
    //                    SeatNumber = $"{rowLabel}{c}",
    //                    SeatIndex = c
    //                });
    //            }
    //        }

    //        await _unitOfWork.Seat.AddRangeAsync(seatsToGenerate);
    //        await _unitOfWork.SaveAsync();
    //    }

    //    // Return the full details DTO
    //    return _mapper.Map<ScreenDetailsDTO>(screen);
    //}

}
}


