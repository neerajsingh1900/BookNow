using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using BookNow.Application.RepoInterfaces; 
using BookNow.Application.DTOs.PaymentDTOs;
using System.Linq;

namespace BookNow.Web.Customer.Infrastructure.Filters
{
    public class BookingOwnershipFilter : IAsyncActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingOwnershipFilter(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int bookingId = 0;

            if (string.IsNullOrEmpty(userId))
            {
                
                context.Result = new UnauthorizedResult();
                return;
            }

          
            if (context.ActionArguments.ContainsKey("bookingId"))
            {
                if (int.TryParse(context.ActionArguments["bookingId"]?.ToString(), out int id))
                {
                    bookingId = id;
                }
            }
            else if (context.ActionArguments.Values.FirstOrDefault() is GatewayResponseDTO dto)
            {
                bookingId = dto.BookingId;
            }

            if (bookingId == 0)
            {
                await next();
                return;
            }

            bool isOwner = await _unitOfWork.Booking.AnyAsync(b =>
                b.BookingId == bookingId && b.UserId == userId);

            if (!isOwner)
            {
                 context.Result = new NotFoundResult();
                return;
            }

            await next();
        }
    }
}