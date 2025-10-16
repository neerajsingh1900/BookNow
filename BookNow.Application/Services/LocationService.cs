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

        public LocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            // Use the generic repository exposed by UnitOfWork.Country
            return await _unitOfWork.Country.GetAllAsync(orderBy: q => q.OrderBy(c => c.Name));
        }

        public async Task<IEnumerable<City>> GetCitiesByCountryIdAsync(int countryId)
        {
            // Use the generic repository exposed by UnitOfWork.City
            return await _unitOfWork.City.GetAllAsync(
                filter: c => c.CountryId == countryId,
                orderBy: q => q.OrderBy(c => c.Name)
            );
        }
    }
}
