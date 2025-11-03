using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using FluentValidation.Results;
using System.Linq;

namespace BookNow.Web.Infrastructure.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           
            var dto = context.ActionArguments.Values.FirstOrDefault();
            if (dto == null)
            {
                await next();
                return;
            }

           
            var validatorType = typeof(IValidator<>).MakeGenericType(dto.GetType());
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validator == null)
            {
                await next();
                return;
            }

            
            var validationContext = new ValidationContext<object>(dto);
            ValidationResult result = await validator.ValidateAsync(validationContext);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                    context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                context.Result = new ViewResult
                {
                    ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                        new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                        context.ModelState)
                    {
                        Model = dto
                    }
                };
                return;
            }

            await next();
        }
    }
}
