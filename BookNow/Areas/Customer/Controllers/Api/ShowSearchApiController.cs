    using BookNow.Application.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace BookNow.Areas.Customer.Controllers.Api
    {
        [Area("Customer")]
        [Route("api/[area]/[controller]")]
        [ApiController]
        public class ShowSearchApiController : Controller
        {

            private readonly IShowSearchService _showSearchService;

            public ShowSearchApiController(IShowSearchService showSearchService)
            {
                _showSearchService = showSearchService;
            }

            [HttpGet("showtimes/{movieId:int}")]
            public async Task<IActionResult> GetShowtimesByDate(int movieId, [FromQuery] string date)
            {
                var cityId = HttpContext.Items["CityId"] as int?;

                if (!cityId.HasValue || !DateOnly.TryParse(date, out DateOnly targetDate))
                {
                    return BadRequest("Invalid location or date parameters.");
                }

                var filteredTheatres = await _showSearchService.GetFilteredShowtimesForDateAsync(movieId,cityId.Value,targetDate);

                return PartialView("~/Views/Shared/_TheatreShowtimesPartial.cshtml", filteredTheatres);
            }
        }
    }
