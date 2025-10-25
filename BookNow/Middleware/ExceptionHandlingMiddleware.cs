using BookNow.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace BookNow.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred for request {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Determine if this is an HTML request (Razor) or API request
            var isHtmlRequest = context.Request.Headers["Accept"].ToString().Contains("text/html") &&
                                !context.Request.Path.StartsWithSegments("/api");

            string errorMessage = exception switch
            {
                ValidationException ve => ve.Message,
                ApplicationValidationException ave => ave.Message,
                UnauthorizedAccessException _ => "User is not authorized.",
                _ => "An unexpected error occurred."
            };

            if (isHtmlRequest)
            {
                // Inject the error into Context.Items so the view can show it
                context.Items["ValidationError"] = errorMessage;

                // Let MVC render the view for the current request
                var endpoint = context.GetEndpoint();
                var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

                if (actionDescriptor != null)
                {
                    var viewResult = new ViewResult
                    {
                        ViewName = actionDescriptor.ActionName,
                        ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
                        {
                            Model = context.Request.HasFormContentType ? context.Request.Form : null
                        }
                    };

                    var actionContext = new ActionContext
                    {
                        HttpContext = context,
                        RouteData = context.GetRouteData(),
                        ActionDescriptor = actionDescriptor
                    };

                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // Show 400 for validation errors
                    await viewResult.ExecuteResultAsync(actionContext);
                    return;
                }
            }

            // For API / AJAX requests, return JSON
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                ValidationException => (int)HttpStatusCode.BadRequest,
                ApplicationValidationException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new { error = errorMessage, code = context.Response.StatusCode };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
