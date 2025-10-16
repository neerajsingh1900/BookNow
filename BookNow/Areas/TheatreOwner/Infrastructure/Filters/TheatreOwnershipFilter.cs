using BookNow.Application.Interfaces; // Dependency on Application Layer
using BookNow.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters
{
    /// <summary>
    /// Custom Action Filter to verify that the logged-in Theatre Owner has ownership
    /// of the TheatreId specified in the request (route or body).
    /// </summary>
    public class TheatreOwnershipFilter : IAsyncActionFilter
    {
        private readonly ITheatreService _theatreService;

        public TheatreOwnershipFilter(ITheatreService theatreService)
        {
            _theatreService = theatreService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Authorization check
            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !context.HttpContext.User.IsInRole(SD.Role_Theatre_Owner))
            {
                context.Result = new ForbidResult();
                return;
            }

            // 2. Extract TheatreId from the request
            int theatreId = 0;

            // Try extracting from route/query parameters (Keys: theatreId or id)
            if (context.ActionArguments.TryGetValue("theatreId", out var routeIdObj) && routeIdObj is int routeId && routeId > 0)
            {
                theatreId = routeId;
            }
            else if (context.ActionArguments.TryGetValue("id", out var idObj) && idObj is int id && id > 0)
            {
                theatreId = id;
            }
            else if (context.ActionArguments.Count > 0)
            {
                // Try extracting from the request body ViewModel/DTO
                var arg = context.ActionArguments.Values.FirstOrDefault();
                if (arg != null)
                {
                    // Look for TheatreId property on the incoming ViewModel
                    PropertyInfo? theatreIdProperty = arg.GetType().GetProperty("TheatreId");
                    if (theatreIdProperty?.GetValue(arg) is int bodyId && bodyId > 0)
                    {
                        theatreId = bodyId;
                    }
                }
            }

            // 3. If no TheatreId was conclusively found, proceed (e.g., for Theatre List or Add Theatre POST).
            if (theatreId == 0)
            {
                await next();
                return;
            }

            // 4. Validate Ownership via the Application Service (Clean Architecture Boundary)
            bool isOwner = await _theatreService.IsOwnerOfTheatreAsync(userId, theatreId);

            if (!isOwner)
            {
                // Return 403 Forbidden if ownership fails
                context.Result = new ForbidResult();
                return;
            }

            // 5. Success, proceed to controller action
            await next();
        }
    }
}
