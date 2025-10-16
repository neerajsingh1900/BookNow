using Microsoft.AspNetCore.Mvc;

namespace BookNow.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("Home/Error")]
        public IActionResult Error()
        {
            var message = TempData["ErrorMessage"] as string
                          ?? "An unexpected error occurred.";
            ViewData["ErrorMessage"] = message;
            return View();
        }
    }
}
