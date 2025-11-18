using BookNow.Application.DTOs.Analytics;
using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq; 

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

        [HttpGet]
        public async Task<IActionResult> RevenueByCountry()
        {
            ViewBag.SelectedCurrency ??= "USD";
            return View(new ProducerAnalyticsInputDto {});
        }

        [HttpGet("Producer/Analytics/StackedRevenueData")]
        [Produces("application/json")]
        public async Task<IActionResult> GetStackedRevenueData([FromQuery] string currency)
        {
            if (string.IsNullOrEmpty(currency))
            {
                return BadRequest("Currency is required.");
            }

            var producerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reportData = await _analyticsService.GetStackedRevenueDataAsync(producerUserId, currency);

            if (reportData == null || !reportData.Any())
            {
                return Ok(Enumerable.Empty<MovieRevenueStackedDto>());
            }

            return Json(reportData);
        }

    }
}