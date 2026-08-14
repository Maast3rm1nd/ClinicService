using Microsoft.AspNetCore.Mvc.Filters;
using ClinicServiceBase.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using WS_ClinicService.Contracts.Responses;

namespace WS_ClinicService.Core.Extensions
{
    public class ApiExceptions : IExceptionFilter
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
                _ => (StatusCodes.Status500InternalServerError, exception.Message)
            };

            context.Result = new ObjectResult(new ErrorResponse
            {
                Code = statusCode,
                Message = string.IsNullOrWhiteSpace(message) ? "Internal Server Error" : message,
                Details = exception.StackTrace
            })
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}