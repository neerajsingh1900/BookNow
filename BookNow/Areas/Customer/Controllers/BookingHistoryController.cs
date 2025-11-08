using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace BookNow.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize] // Requires user to be logged in to access this page
    public class BookingHistoryController : Controller
    {
        private readonly IBookingHistoryService _historyService;

        public BookingHistoryController(IBookingHistoryService historyService)
        {
            _historyService = historyService;
        }

        // GET: /Customer/BookingHistory/Index
        public async Task<IActionResult> Index()
        {
            // Get the ID of the currently logged-in user securely
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // Should not happen with [Authorize], but safe to redirect to login
                return Redirect("/Identity/Account/Login");
            }

            // Call the service to get all confirmed bookings, sorted
            var history = await _historyService.GetFullHistoryAsync(userId);

            // The service returns the List<BookingHistoryViewModel> which is passed directly to the view
            return View(history);
        }
    }
}