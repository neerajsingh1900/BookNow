using System.ComponentModel.DataAnnotations;

namespace BookNow.Web.Areas.TheatreOwner.ViewModels.Screen
{
    public class ScreenUpsertVM
    {
        public int? ScreenId { get; set; }

        [Required]
        public int TheatreId { get; set; } 

        [Required(ErrorMessage = "Screen number/name is required.")]
        [StringLength(50)]
        [Display(Name = "Screen Number/Name")]
        public string ScreenNumber { get; set; } = null!;

        [Required(ErrorMessage = "Number of rows is required.")]
        [Range(1, 50, ErrorMessage = "Number of rows must be between 1 and 50.")]
        [Display(Name = "Number of Rows (A, B, C...)")]
        public int NumberOfRows { get; set; }

        [Required(ErrorMessage = "Seats per row is required.")]
        [Range(1, 100, ErrorMessage = "Seats per row must be between 1 and 100.")]
        [Display(Name = "Seats per Row (1, 2, 3...)")]
        public int SeatsPerRow { get; set; }

        [Required(ErrorMessage = "Default price is required.")]
        [Range(0.01, 99999.99, ErrorMessage = "Price must be greater than zero.")]
        [Display(Name = "Default Seat Price")]
        public decimal DefaultSeatPrice { get; set; }
    }
}
