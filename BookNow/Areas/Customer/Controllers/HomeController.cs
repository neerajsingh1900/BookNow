// BookNow.Web/Areas/Customer/Controllers/HomeController.cs

using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookNow.Web.Areas.Customer.Controllers
{
    // Designate this controller for the Customer Area
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService; // Injecting IMovieService for future use

        public HomeController(ILogger<HomeController> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }

        // GET /Customer/Home/Index
        public IActionResult Index()
        {
            _logger.LogInformation("Loading Customer Home Index.");
            // Later, we will use _movieService to fetch current movies for the homepage
            return View();
        }

        // GET /Customer/Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // GET /Customer/Home/About
       
    }
}
