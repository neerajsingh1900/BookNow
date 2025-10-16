using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookNow.Web.Areas.TheatreOwner.ViewModels.Screen
{
    /// <summary>
    /// Minimal ViewModel used to render the visual details of a screen's capacity.
    /// </summary>
    public class SeatLayoutGridVM
    {
        public int ScreenId { get; set; }
        public string ScreenNumber { get; set; } = null!;

        [Display(Name = "Total Seats")]
        public int TotalSeats { get; set; }

        [Display(Name = "Price")]
        public decimal DefaultSeatPrice { get; set; }

        [Display(Name = "Rows")]
        public int NumberOfRows { get; set; }

        [Display(Name = "Seats/Row")]
        public int SeatsPerRow { get; set; }

        // This is where a detailed seat map structure would go if rendering individual seats.
    }
}
