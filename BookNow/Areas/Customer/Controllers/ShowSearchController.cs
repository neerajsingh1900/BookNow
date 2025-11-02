using BookNow.Application.Interfaces;
using BookNow.Application.DTOs.CustomerDTOs.SearchDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace BookNow.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShowSearchController : Controller
    {
        private readonly IShowSearchService _showSearchService;

        public ShowSearchController(IShowSearchService showSearchService)
        {
            _showSearchService = showSearchService;
        }

        public async Task<IActionResult> SelectTheatre(int movieId)
        {
            var cityId = HttpContext.Items["CityId"] as int?;

            if (!cityId.HasValue) return RedirectToAction("Index", "Home");

            var pageData = await _showSearchService.GetShowtimesForWindowAsync(movieId, cityId.Value);

            if (pageData == null || pageData.Movie == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(pageData);
        }
    }
}
