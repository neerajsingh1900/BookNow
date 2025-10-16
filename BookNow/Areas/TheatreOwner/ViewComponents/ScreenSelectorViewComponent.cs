using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.ViewComponents
{
    /// <summary>
    /// Renders a reusable dropdown/selector for screens belonging to a theatre.
    /// </summary>
    public class ScreenSelectorViewComponent : ViewComponent
    {
        private readonly ITheatreService _theatreService;

        public ScreenSelectorViewComponent(ITheatreService theatreService)
        {
            _theatreService = theatreService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int theatreId, int? selectedScreenId = null)
        {
            // Note: Ownership check for theatreId should occur upstream via TheatreOwnershipFilter

            var screens = await _theatreService.GetTheatreScreensAsync(theatreId);

            var screenList = screens.Select(s => new SelectListItem
            {
                Value = s.ScreenId.ToString(),
                Text = s.ScreenNumber,
                Selected = (s.ScreenId == selectedScreenId)
            }).ToList();

            // Returns List<SelectListItem> to the component view for rendering a <select>
            return View(screenList);
        }
    }
}
