using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using BookNow.Application.Interfaces;
using BookNow.Application.Services.Booking;
using BookNow.Utility;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

[Area("Customer")]
public class BookingController : Controller
{
    private readonly ISeatBookingService _bookingService;

    public BookingController(ISeatBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    private int GetCityId() =>
    HttpContext.Items.TryGetValue("CityId", out var obj) && obj is int id ? id : 1;


    [HttpGet]
    [Route("Customer/Booking/SeatLayout/{showId:int}")]
    public async Task<IActionResult> SeatLayout(int showId)
    {
        var model = await _bookingService.GetSeatLayoutAsync(showId, GetCityId());
        ViewData["Title"] = $"Select Seats for {model.MovieTitle}";
        return View(model);
    }

}
