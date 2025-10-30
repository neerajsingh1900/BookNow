using BookNow.Application.DTOs.CommonDTOs;
using BookNow.Application.DTOs.CustomerDTOs.SearchDTOs;
using BookNow.Application.Interfaces;
using BookNow.Areas.Customer.ViewModels;
using BookNow.Web.ViewModels; 
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks; 

namespace BookNow.Web.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IShowSearchService _showSearchService;

        public HomeController(IShowSearchService showSearchService)
        {
            _showSearchService = showSearchService;
        }


        public async Task<IActionResult> Index()
        {
            var cityId = HttpContext.Items["CityId"] as int?;
            IEnumerable<MovieListingDTO> movieModel = new List<MovieListingDTO>();

            if (cityId.HasValue)
            {
                movieModel = await _showSearchService.GetMoviesByCityAsync(cityId);

            }

            return View(movieModel); 
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}