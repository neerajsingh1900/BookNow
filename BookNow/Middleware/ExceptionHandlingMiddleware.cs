using BookNow.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace BookNow.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case ApplicationValidationException notFoundEx:
                    status = HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    break;
                case ValidationException validationEx:
                    status = HttpStatusCode.BadRequest;
                    message = validationEx.Message;
                    break;
                case UnauthorizedAccessException _:
                    status = HttpStatusCode.Unauthorized;
                    message = "User is not authorized.";
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            context.Response.StatusCode = (int)status;
            var response = new { error = message, code = context.Response.StatusCode };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}