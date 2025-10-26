using AutoMapper;
using BookNow.Application.DTOs.CustomerDTOs.SearchDTOs;
using BookNow.Application.Interfaces;
using BookNow.Models.Interfaces;
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
       

    }
}
