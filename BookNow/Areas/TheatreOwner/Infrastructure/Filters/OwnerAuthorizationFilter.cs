using BookNow.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace BookNow.Web.Areas.TheatreOwner.Infrastructure.Filters
{
    /// <summary>
    /// Demonstrative Authorization Filter. In this app, [Authorize(Roles = "TheatreOwner")] 
    /// is simpler, but this shows custom authorization logic implementation.
    /// </summary>
    public class OwnerAuthorizationFilter : IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated ||
                !context.HttpContext.User.IsInRole(SD.Role_Theatre_Owner))
            {
                context.Result = new ForbidResult();
            }
            return Task.CompletedTask;
        }
    }
}
