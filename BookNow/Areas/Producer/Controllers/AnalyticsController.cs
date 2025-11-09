using BookNow.Application.DTOs.Analytics;
using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; 
using System.Threading.Tasks;

namespace BookNow.Web.Areas.Producer.Controllers
{
    [Area("Producer")]
    [Authorize(Roles = "Producer")]
    public class AnalyticsController : Controller
    {
        private readonly IProducerAnalyticsService _analyticsService;

        public AnalyticsController(IProducerAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // GET: /Producer/Analytics/RevenueByCountry - Initial page load action
        [HttpGet]
        public async Task<IActionResult> RevenueByCountry()
        {
            // 🌟 CRITICAL CHANGE 1: Get the logged-in Producer's UserId 🌟
            var producerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            

            // 🌟 CRITICAL CHANGE 2: Pass the UserId to the service to filter the movies 🌟
            var model = await _analyticsService.GetInputDataAsync(producerUserId);

            // Set defaults for the view 
            ViewBag.SelectedCurrency ??= "INR";

            // Note: We use the DTO model structure for view presentation
            return View(model);
        }

        // API Endpoint for AJAX requests (Unchanged, as it already accepts movieId and currency)
        [HttpGet("Producer/Analytics/RevenueData")]
        [Produces("application/json")]
        public async Task<IActionResult> GetRevenueData([FromQuery] int movieId, [FromQuery] string currency)
        {
            if (movieId <= 0 || string.IsNullOrEmpty(currency))
            {
                return BadRequest("Movie ID and Currency are required.");
            }
                
            var reportData = await _analyticsService.GetRevenueByCountryAsync(movieId, currency);

            return Json(reportData);
        }
    }
}