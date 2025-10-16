using BookNow.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookNow.Application.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<Country>> GetAllCountriesAsync();
        Task<IEnumerable<City>> GetCitiesByCountryIdAsync(int countryId);
    }
}
