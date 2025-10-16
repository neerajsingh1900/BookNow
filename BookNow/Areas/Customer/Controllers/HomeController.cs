using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookNow.Web.ViewModels; 
using System.Linq;
using System.Threading.Tasks; 

namespace BookNow.Web.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;

        // [C# Showcase: Dependency Injection] Correctly receives all dependencies
        public HomeController(ILogger<HomeController> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }

       
        public async Task<IActionResult> Index()
        {
          
            var movieReadDTOs = await _movieService.GetAllMoviesAsync();

            
            var movies = movieReadDTOs
                .Select(m => new CustomerMovieViewModel
                {
                    MovieId = m.MovieId,
                    Title = m.Title,
                    Genre = m.Genre,
                    Language = m.Language,
                    Duration = m.Duration,
                    PosterUrl = m.PosterUrl,
                    ReleaseDate = m.ReleaseDate
                }).ToList();

            _logger.LogInformation("Successfully retrieved and mapped {MovieCount} movies for the public index.", movies.Count);

            return View(movies);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}