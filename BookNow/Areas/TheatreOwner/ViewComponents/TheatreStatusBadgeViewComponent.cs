using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.ViewComponents
{
    
    public class TheatreStatusBadgeViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string status)
        {
            // Passes the status string directly to its corresponding view (Default.cshtml)
            return View((object)status);
        }
    }
}
