using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookNow.Web.ViewModels;

namespace BookNow.Web.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;

        public HomeController(ILogger<HomeController> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }

       
        public IActionResult Index()
        {
            var movies = _movieService.GetAllMovies()
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

            return View(movies);
        }

       
        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
