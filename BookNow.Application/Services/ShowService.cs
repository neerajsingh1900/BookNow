using AutoMapper;
using BookNow.Application.DTOs.ShowDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.Validation.ScreenValidations;
using BookNow.Models.Interfaces;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
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
        public async Task<ScreenShowListDTO> GetScreenShowsByOwnerAsync(int screenId, string ownerId)
        {
            
            var screenWithShows = await _unitOfWork.Screen.GetAsync(
              
                filter: s => s.ScreenId == screenId && s.Theatre.OwnerId == ownerId,
                includeProperties: "Theatre,Shows.Movie"
            );
                
            if (screenWithShows == null)
                throw new NotFoundException($"Screen ID {screenId} not found or access denied.");

            var showListDto = _mapper.Map<ScreenShowListDTO>(screenWithShows);

            return showListDto;
        }
    }
}
