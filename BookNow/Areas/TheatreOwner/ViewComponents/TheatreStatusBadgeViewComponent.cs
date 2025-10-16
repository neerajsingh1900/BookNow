using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.ViewComponents
{
    /// <summary>
    /// Renders a styled badge based on the theatre status string.
    /// The logic for choosing the color/text is contained in the component's View.
    /// </summary>
    public class TheatreStatusBadgeViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string status)
        {
            // Passes the status string directly to its corresponding view (Default.cshtml)
            return View((object)status);
        }
    }
}
