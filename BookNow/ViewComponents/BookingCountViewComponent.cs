using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;

namespace BookNow.Web.ViewComponents
{
    // Make sure to register this ViewComponent in your Startup.cs or Program.cs if needed (e.g., builder.Services.AddScoped<IBookingHistoryService, BookingHistoryService>();)
    public class BookingCountViewComponent : ViewComponent
    {
        private readonly IBookingHistoryService _historyService;

        public BookingCountViewComponent(IBookingHistoryService historyService)
        {
            _historyService = historyService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // If the user is not authenticated, we render nothing (as requested)
            if (!User.Identity.IsAuthenticated)
            {
                return Content(string.Empty);
            }

            var userId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // Should not happen if authenticated, but a safety check
                return Content(string.Empty);
            }

            // Fetch the count of only confirmed, UPCOMING bookings
            var count = await _historyService.GetUpcomingCountAsync(userId);

            return View(count);
        }
    }
}