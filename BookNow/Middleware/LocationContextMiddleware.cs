using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BookNow.Web.Middleware
{
    public class LocationContextMiddleware
    {
        private readonly RequestDelegate _next;

        private const string CityIdCookieKey = "BN_CityId";
        private const string CityNameCookieKey = "BN_CityName";

        public LocationContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
           
            var cityIdCookie = context.Request.Cookies[CityIdCookieKey];
            var cityNameCookie = context.Request.Cookies[CityNameCookieKey];

            if (int.TryParse(cityIdCookie, out var cityId))
            {
               
                context.Items["CityId"] = cityId;
                context.Items["CityName"] = cityNameCookie;
            }

            
            await _next(context);
        }
    }
}
