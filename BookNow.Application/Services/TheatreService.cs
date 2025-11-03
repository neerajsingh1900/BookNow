using AutoMapper;
using BookNow.Application.DTOs.ScreenDTOs;
using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Application.DTOs.TheatreDTOs;
using BookNow.Application.Exceptions; 
using BookNow.Application.Interfaces;
using BookNow.Application.RepoInterfaces;
using BookNow.Models;
using BookNow.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TheatreService> _logger;
        public TheatreService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TheatreService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TheatreDetailDTO> AddTheatreAsync(string ownerId, TheatreUpsertDTO dto)
        {
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
         
            _logger.LogInformation("Theatre {TheatreId} created successfully for owner {OwnerId}", theatre.TheatreId, ownerId);


            var fullTheatre = await _unitOfWork.Theatre.GetAsync(
                t => t.TheatreId == theatre.TheatreId,
                includeProperties: "City.Country,Screens"); 

            return _mapper.Map<TheatreDetailDTO>(fullTheatre); 
        }
    
        public async Task<TheatreDetailDTO> UpdateTheatreAsync(int theatreId, TheatreUpsertDTO dto, string ownerId)
        {
            
            var theatre = await _unitOfWork.Theatre.GetAsync(t => t.TheatreId == theatreId);

            if (theatre == null)
            {
                _logger.LogWarning("Theatre {TheatreId} not found during update attempt by owner {OwnerId}", theatreId, ownerId);

                throw new NotFoundException($"Theatre with ID {theatreId} not found.");
            }

            if (theatre.OwnerId != ownerId)
            {
             _logger.LogWarning("Unauthorized theatre update attempt. Theatre {TheatreId} not owned by {OwnerId}", theatreId, ownerId);

                throw new UnauthorizedAccessException();
            }

            theatre.UpdateDetails(dto.TheatreName, dto.Address, dto.CityId,
                dto.PhoneNumber, dto.Email);

             _unitOfWork.Theatre.Update(theatre);
            await _unitOfWork.SaveAsync();
         
            _logger.LogInformation("Theatre {TheatreId} updated successfully", theatreId);


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


        public async Task<IEnumerable<Screen>> GetTheatreScreensAsync(int theatreId)
        {
            return await _unitOfWork.Screen.GetScreensByTheatreAsync(theatreId);
        }
        public async Task<int?> GetTheatreIdByScreenIdAsync(int screenId)
        {
            
            var screen = await _unitOfWork.Screen.GetAsync(s => s.ScreenId == screenId);
            return screen?.TheatreId;
        }


        public async Task<IEnumerable<Show>> GetScreenShowsAsync(int screenId)
        {
            var shows = await _unitOfWork.Show.GetAllAsync(
                filter: s => s.ScreenId == screenId,
                orderBy: q => q.OrderBy(s => s.StartTime),
                includeProperties: "Movie"); 

            return shows;
        }
        public async Task<bool> IsOwnerOfTheatreAsync(string userId, int theatreId)
        {
           
            var theatre = await _unitOfWork.Theatre.GetAsync(
                t => t.TheatreId == theatreId && t.OwnerId == userId,
                tracked: false);

            return theatre != null;
        }

    }
}