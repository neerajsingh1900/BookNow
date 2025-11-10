using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace BookNow.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize] 
    public class BookingHistoryController : Controller
    {
        private readonly IBookingHistoryService _historyService;

        public BookingHistoryController(IBookingHistoryService historyService)
        {
            _historyService = historyService;
        }

      
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }

            var history = await _historyService.GetFullHistoryAsync(userId);

            return View(history);
        }
    }
}