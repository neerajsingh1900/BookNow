using BookNow.Application.DTOs.CommonDTOs;
using BookNow.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<CountryDTO>> GetAllCountriesAsync();
        Task<IEnumerable<CityDTO>> GetCitiesByCountryIdAsync(int countryId);
        Task<CityDTO?> GetCityByIdAsync(int cityId);
    }
}
