using ClinicServiceBase.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WS_ClinicService.Contracts.Responses;

namespace WS_ClinicService.Core.Extensions
{
    public class ApiExceptions(ILogger<ApiExceptions> logger) : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            var (statusCode, message) = exception switch
            {
                BadRequestException e => (StatusCodes.Status400BadRequest, e.Message),
                ForbiddenException e => (StatusCodes.Status403Forbidden, e.Message),
                RecordNotFoundException e => (StatusCodes.Status404NotFound, e.Message),
                RequestTimeoutException e => (StatusCodes.Status408RequestTimeout, e.Message),
                ConflictException e => (StatusCodes.Status409Conflict, e.Message),
                UnprocessableEntityException e => (StatusCodes.Status422UnprocessableEntity, e.Message),
                _ => (StatusCodes.Status500InternalServerError, string.Empty)
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
            }

            context.Result = new ObjectResult(new ErrorResponse
            {
                Code = statusCode,
                Message = string.IsNullOrWhiteSpace(message) ? "Internal Server Error" : message
            })
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
