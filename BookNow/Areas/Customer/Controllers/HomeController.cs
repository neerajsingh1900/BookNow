using BookNow.Application.DTOs.CommonDTOs;
using BookNow.Application.Interfaces;
using BookNow.Areas.Customer.ViewModels;
using BookNow.Web.ViewModels; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks; 

namespace BookNow.Web.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
       
        private readonly ILocationService _locationService;

        // [C# Showcase: Dependency Injection] Correctly receives all dependencies
        public HomeController(ILogger<HomeController> logger
            , ILocationService locationService)
        {
            _logger = logger;
          
            _locationService = locationService;
        }


        public async Task<IActionResult> Index()
        {
            var countries = await _locationService.GetAllCountriesAsync();

            // Map to DTO (clean separation)
            var countryDtos = countries.Select(c => new CountryDTO
            {
                CountryId = c.CountryId,
                Name = c.Name,
                Code = c.Code
            }).ToList();

            
            return View(countryDtos);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}