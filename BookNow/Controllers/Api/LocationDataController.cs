using BookNow.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

[Route("api/[controller]")]
[ApiController]
public class LocationDataController : ControllerBase
{
    private const string CityIdCookieKey = "BN_CityId";
    private const string CityNameCookieKey = "BN_CityName";
    private readonly ILocationService _locationService;

    public LocationDataController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpPost("set")]
   
    public async Task<IActionResult> SetLocation([FromForm] int cityId, [FromForm] string cityName)
    {
        if (cityId <= 0 || string.IsNullOrEmpty(cityName))
            return BadRequest("Invalid location data provided.");
        
        var city = await _locationService.GetCityByIdAsync(cityId);
        if (city == null)
            return NotFound("City not found.");

        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(30),
            IsEssential = true,
            HttpOnly = false, 
            Secure = true, 
            SameSite = SameSiteMode.Lax
        };

        Response.Cookies.Append(CityIdCookieKey, cityId.ToString(), cookieOptions);
        Response.Cookies.Append(CityNameCookieKey, cityName, cookieOptions);

       return Ok(new { success = true });
    }
}