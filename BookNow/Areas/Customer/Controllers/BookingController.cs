
using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using BookNow.Application.Services.Booking;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using BookNow.Application.Interfaces;

[Area("Customer")]
public class BookingController : Controller
{
    private readonly ISeatBookingService _bookingService;

    public BookingController(ISeatBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    [Route("Customer/Booking/SeatLayout/{showId:int}")]
    public async Task<IActionResult> SeatLayout(int showId)
    {
        if (!HttpContext.Items.TryGetValue("CityId", out object cityIdObj) || !(cityIdObj is int cityId))
        {
            cityId = 1;
        }

        try
        {
            var model = await _bookingService.GetSeatLayoutAsync(showId, cityId);

            ViewData["Title"] = $"Select Seats for {model.MovieTitle}";
            return View(model);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Error loading seat map. Please try again later.");
        }
    }

    [HttpPost]
    [Route("Customer/Booking/CreateHold")]
    public async Task<IActionResult> CreateHoldAndRedirect([FromBody] CreateHoldCommandDTO command)
    {
      
        if (!User.Identity!.IsAuthenticated)
        {
            return Unauthorized(new { error = "Login required to proceed with booking." });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userEmail = User.FindFirstValue(ClaimTypes.Email)!;

        var result = await _bookingService.CreateTransactionalHoldAsync(command, userId, userEmail);

        if (!result.Success)
        {
             return Conflict(new { error = result.ErrorMessage ?? "Concurrency error or validation failed." });
        }

        return Json(new { success = true, redirectUrl = result.RedirectUrl });
    }
}
