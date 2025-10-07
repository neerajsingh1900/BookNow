using System.ComponentModel.DataAnnotations;
using BookNow.ViewModels.Shared; // Reference the shared ViewModels namespace

namespace BookNow.ViewModels.Theatre // Assuming this is the correct namespace
{
    public class CreateTheatreViewModel
    {
        // --- Input Fields ---

        [Required(ErrorMessage = "Theatre Name is required.")]
        [StringLength(100, ErrorMessage = "Theatre Name cannot exceed 100 characters.")]
        public string TheatreName { get; set; } = null!;

        // The Country is selected for the dynamic filtering, but only the CityId is saved.
        [Required(ErrorMessage = "Country selection is required.")]
        public string CountryCode { get; set; } = null!; // Stored as Code (e.g., "IND") for AJAX filtering

        [Required(ErrorMessage = "City selection is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid City.")]
        public int CityId { get; set; } // The final ID sent to the service layer

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255)]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Phone Number is required.")]
        [StringLength(15)]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = null!;

        // --- Data for View ---

        // Holds the lists needed to populate dropdowns on the initial GET request
        public CreateTheatreDataViewModel? DropdownData { get; set; }
    }
}