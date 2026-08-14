using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WS_ClinicService.Contracts.Responses;

namespace WS_ClinicService.Core.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var errors = new List<string>();
            var services = context.HttpContext.RequestServices;

            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null)
                {
                    continue;
                }

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

                if (services.GetService(validatorType) is IValidator validator)
                {
                    var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument));

                    errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage));
                }
            }

            if (errors.Count > 0)
            {
                context.Result = new ObjectResult(new ErrorResponse
                {
                    Code = StatusCodes.Status422UnprocessableEntity,
                    Message = "Validation failed",
                    Details = string.Join("; ", errors)
                })
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };

                return;
            }

            await next();
        }
    }
}