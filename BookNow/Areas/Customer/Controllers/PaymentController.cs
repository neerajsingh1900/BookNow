using BookNow.Application.DTOs.PaymentDTOs;
using BookNow.Application.Interfaces;
using BookNow.Utility;
using BookNow.Web.Customer.Infrastructure.Filters;
using BookNow.Web.Infrastructure.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.Customer.Controllers 
{
   
    [Area("Customer")]

   
    [Authorize(Roles = SD.Role_Customer)]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        
        private int GetCityId() =>
            HttpContext.Items.TryGetValue("CityId", out var obj) && obj is int id ? id : 1;

        [ServiceFilter(typeof(BookingOwnershipFilter))]
        [HttpGet]
        public async Task<IActionResult> Gateway(int bookingId)
        {
            var summary = await _paymentService.GetBookingSummaryAsync(bookingId, GetCityId());

            if (summary == null)
            {
               
                return RedirectToAction("Timeout");
            }

            return View(summary);
        }

      
        [ServiceFilter(typeof(BookingOwnershipFilter))]
        [HttpPost]
        public async Task<IActionResult> HandleGatewayResponse([FromBody] GatewayResponseDTO response)
        {
           
            string redirectActionName = await _paymentService.ProcessGatewayResponseAsync(response);

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action(redirectActionName, "Payment", new { area = "Customer" })
            });
        }
       
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ReleaseSeats([FromBody] GatewayResponseDTO? response)
        {
            if (response == null || response.BookingId <= 0)
                return Ok(); 
            await _paymentService.ReleaseSeatsAndLocksAsync(response.BookingId);
            return Ok();
        }

      
        [AllowAnonymous]
        public IActionResult Success() => View();

       
        [AllowAnonymous]
        public IActionResult Failed() => View();

        
        [AllowAnonymous]
        public IActionResult Timeout() => View();
    }
}