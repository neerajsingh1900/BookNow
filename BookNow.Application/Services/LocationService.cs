using AutoMapper;
using BookNow.Application.DTOs.CommonDTOs;
using BookNow.Application.Interfaces;
using BookNow.Models;
using BookNow.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CountryDTO>> GetAllCountriesAsync()
        {
            var countries = await _unitOfWork.Country.GetAllAsync(orderBy: q => q.OrderBy(c => c.Name));
            return _mapper.Map<IEnumerable<CountryDTO>>(countries);
        }

        public async Task<IEnumerable<CityDTO>> GetCitiesByCountryIdAsync(int countryId)
        {
            var cities = await _unitOfWork.City.GetAllAsync(
                filter: c => c.CountryId == countryId,
                orderBy: q => q.OrderBy(c => c.Name)
            );
            return _mapper.Map<IEnumerable<CityDTO>>(cities);
        }
        public async Task<CityDTO?> GetCityByIdAsync(int cityId)
        {
            var city = (await _unitOfWork.City.GetAllAsync(filter: c => c.CityId == cityId)).FirstOrDefault();
            return _mapper.Map<CityDTO?>(city);
        }
    }
}
