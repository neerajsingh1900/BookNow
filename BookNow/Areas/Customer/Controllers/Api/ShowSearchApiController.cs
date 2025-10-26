using BookNow.Application.Interfaces;
using BookNow.Application.DTOs.CustomerDTOs.SearchDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.Customer.Controllers.Api
{
    [Area("Customer")]
    [Route("api/movies")]
    [ApiController]
    public class ShowSearchApiController : ControllerBase
    {
        private readonly IShowSearchService _showSearchService;

        public ShowSearchApiController(IShowSearchService showSearchService)
        {
            _showSearchService = showSearchService;
        }

        // GET: /api/movies/bycity/{cityId}
        [HttpGet("bycity/{cityId:int}")]
        public async Task<IActionResult> GetMoviesByCity(int cityId)
        {
            var movies = await _showSearchService.GetMoviesByCityAsync(cityId);

            //var result = movies
            //    .Select(m => new MovieListingDTO
            //    {
            //        MovieId = m.MovieId,
            //        Title = m.Title,
            //        Genre = m.Genre ?? "",
            //        Language = m.Language ?? "",
            //        Duration = m.Duration,
            //        PosterUrl = m.PosterUrl ?? "/images/default-poster.png"
            //    })
            //    .ToList();

            return Ok(movies);
        }
    }
}
