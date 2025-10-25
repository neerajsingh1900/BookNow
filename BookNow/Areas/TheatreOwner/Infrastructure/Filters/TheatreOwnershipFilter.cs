using BookNow.Application.Interfaces; 
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
    
    public class TheatreOwnershipFilter : IAsyncActionFilter
    {
        private readonly ITheatreService _theatreService;

        public TheatreOwnershipFilter(ITheatreService theatreService)
        {
            _theatreService = theatreService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !context.HttpContext.User.IsInRole(SD.Role_Theatre_Owner))
            {
                context.Result = new ForbidResult();
                return;
            }

            int theatreId = 0;

            
             if (context.ActionArguments.TryGetValue("theatreId", out var routeIdObj) && routeIdObj is int routeId && routeId > 0)
            {
                theatreId = routeId;
            }
            else if (context.ActionArguments.TryGetValue("screenId", out var screenIdObj) && screenIdObj is int routeScreenId && routeScreenId > 0)
            {

                int? owningTheatreId = await _theatreService.GetTheatreIdByScreenIdAsync(routeScreenId);
                if (owningTheatreId.HasValue)
                {
                    theatreId = owningTheatreId.Value;
                }
            }
            else if (context.ActionArguments.TryGetValue("id", out var idObj) && idObj is int id && id > 0)
            {
                int? owningTheatreId = await _theatreService.GetTheatreIdByScreenIdAsync(id);
                if (owningTheatreId.HasValue)
                {
                    theatreId = owningTheatreId.Value;
                }
            }
            else if (context.ActionArguments.Count > 0)
            {
                var arg = context.ActionArguments.Values.FirstOrDefault();
                if (arg != null)
                {
                     PropertyInfo? theatreIdProperty = arg.GetType().GetProperty("TheatreId");
                    if (theatreIdProperty?.GetValue(arg) is int bodyId && bodyId > 0)
                    {
                        theatreId = bodyId;
                    }
                }
            }

            if (theatreId == 0)
            {
                await next();
                return;
            }

            bool isOwner = await _theatreService.IsOwnerOfTheatreAsync(userId, theatreId);

            if (!isOwner)
            {
               
                context.Result = new ForbidResult();
                return;
            }

           
            await next();
        }
    }
}
