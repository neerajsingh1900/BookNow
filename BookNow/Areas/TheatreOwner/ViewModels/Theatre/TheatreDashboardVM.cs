using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Web.Areas.TheatreOwner.ViewModels.Theatre
{
    /// <summary>
    /// ViewModel for a single theatre entry in the dashboard list (Read Model).
    /// </summary>
    public class TheatreListItemVM
    {
        public int TheatreId { get; set; }
        public string TheatreName { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string CountryName { get; set; } = null!;
        public string Status { get; set; } = null!;

        [Display(Name = "Screens")]
        public int ScreenCount { get; set; }
    }

    /// <summary>
    /// Aggregation ViewModel for the main Theatre Index/Dashboard View.
    /// </summary>
    public class TheatreDashboardVM
    {
        public List<TheatreListItemVM> OwnerTheatres { get; set; } = new List<TheatreListItemVM>();
        public int PendingApprovalCount { get; set; }
    }
}
