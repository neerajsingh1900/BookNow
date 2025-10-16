using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.Web.Controllers.Api
{
    // This API is outside any secured area, allowing access for registration forms
    [Route("api/location")]
    [ApiController]
    public class LocationApiController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationApiController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // GET: api/location/countries
        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _locationService.GetAllCountriesAsync();
            // Projecting to anonymous objects for minimal payload size
            return Ok(countries.Select(c => new { id = c.CountryId, name = c.Name }));
        }

        // GET: api/location/cities/5
        [HttpGet("cities/{countryId:int}")]
        public async Task<IActionResult> GetCities(int countryId)
        {
            if (countryId <= 0)
            {
                return BadRequest("Invalid Country ID.");
            }

            var cities = await _locationService.GetCitiesByCountryIdAsync(countryId);
            return Ok(cities.Select(c => new { id = c.CityId, name = c.Name }));
        }
    }
}
