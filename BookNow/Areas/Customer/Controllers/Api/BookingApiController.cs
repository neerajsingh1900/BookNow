using System.Security.Claims;
using BookNow.Application.Interfaces;
using BookNow.Utility;
using BookNow.Application.DTOs.CustomerDTOs.BookingDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookNow.Application.Services.Booking;

namespace BookNow.Areas.Customer.Controllers.Api
{
    [Area("Customer")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_Customer)]
    public class BookingApiController : ControllerBase
    {
        private readonly ISeatBookingService _bookingService;

        public BookingApiController(ISeatBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        [HttpPost("CreateHold")]
        public async Task<IActionResult> CreateHold([FromBody] CreateHoldCommandDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userEmail = User.FindFirstValue(ClaimTypes.Email)!;

            var result = await _bookingService.CreateTransactionalHoldAsync(dto, userId, userEmail);

            return result.Success
                ? Ok(new { success = true, redirectUrl = result.RedirectUrl })
                : Conflict(new { error = result.ErrorMessage ?? "Concurrency error or validation failed." });
        }
    }
}
