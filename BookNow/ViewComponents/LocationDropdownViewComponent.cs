using BookNow.Application.Interfaces;
using BookNow.Application.DTOs.CommonDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

public class LocationDropdownViewComponent : ViewComponent
{
    private readonly ILocationService _locationService;

    public LocationDropdownViewComponent(ILocationService locationService)
    {
        _locationService = locationService;
    }


    public async Task<IViewComponentResult> InvokeAsync()
    {
        var countries = await _locationService.GetAllCountriesAsync();

        var selectedCityId = HttpContext.Items["CityId"];
        var selectedCityName = HttpContext.Items["CityName"] as string;

        ViewData["SelectedCityId"] = selectedCityId;
        ViewData["SelectedCityName"] = selectedCityName;

        return View(countries);
    }
}