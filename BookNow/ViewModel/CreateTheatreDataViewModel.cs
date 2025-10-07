using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BookNow.ViewModels.Shared // Assuming this is the correct namespace
{
    public class CreateTheatreDataViewModel
    {
        // SelectListItem is the standard ASP.NET Core MVC type for dropdown options.
        public IEnumerable<SelectListItem> Countries { get; set; } = new List<SelectListItem>();
    }

    // This DTO is returned by the API controller (ApiController.cs) and consumed by JavaScript.
    // Ensure you have defined this DTO structure in your DTOs folder:
    // public class CityDTO { public int CityId { get; set; } public string Name { get; set; } }
}