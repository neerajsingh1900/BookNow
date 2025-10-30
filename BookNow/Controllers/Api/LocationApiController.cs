using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.Web.Controllers.Api
{
   
    [Route("api/location")]
    [ApiController]
    public class LocationApiController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationApiController(ILocationService locationService)
        {
            _locationService = locationService;
        }

       
        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _locationService.GetAllCountriesAsync();
            return Ok(countries);
        }

       
        [HttpGet("cities/{countryId:int}")]
        public async Task<IActionResult> GetCities(int countryId)
        {
            if (countryId <= 0)
                return BadRequest("Invalid Country ID.");

            var cities = await _locationService.GetCitiesByCountryIdAsync(countryId);
            return Ok(cities);
        }
    }
}
