using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookNow.Web.Areas.TheatreOwner.ViewModels.Theatre
{
    /// <summary>
    /// ViewModel for the Theatre Create/Edit form (Write Model).
    /// </summary>
    public class TheatreUpsertVM
    {
        public int? TheatreId { get; set; } // Null when creating

        [Required(ErrorMessage = "Theatre Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Theatre Name")]
        public string TheatreName { get; set; } = null!;

        [Required(ErrorMessage = "City is required.")]
        [Display(Name = "City")]
        public int CityId { get; set; } // Foreign key for City

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250)]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(15)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Contact Email")]
        public string Email { get; set; } = null!;

        // UI helper property for City selection dropdown
        public IEnumerable<SelectListItem>? CityList { get; set; }
    }
}
