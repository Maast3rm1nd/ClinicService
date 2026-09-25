using ClinicServiceBase.Common.Exceptions;
using System.Text.Json;
using WS_ClinicService.Contracts.Responses;

namespace WS_ClinicService.Core.Middleware
{
    public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
                await WriteErrorResponseAsync(context, exception);
            }
        }

        private static async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            var (statusCode, message) = exception switch
            {
                BadRequestException e => (StatusCodes.Status400BadRequest, e.Message),
                ForbiddenException e => (StatusCodes.Status403Forbidden, e.Message),
                RecordNotFoundException e => (StatusCodes.Status404NotFound, e.Message),
                RequestTimeoutException e => (StatusCodes.Status408RequestTimeout, e.Message),
                ConflictException e => (StatusCodes.Status409Conflict, e.Message),
                UnprocessableEntityException e => (StatusCodes.Status422UnprocessableEntity, e.Message),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse
            {
                Code = statusCode,
                Message = message
            }));
        }
    }
}
