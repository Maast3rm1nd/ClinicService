using Microsoft.AspNetCore.Mvc.Filters;
using ClinicServiceBase.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;


namespace WS_ClinicService.Core.Extensions
{
    public class ApiExceptions : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case BadRequestException badRequest:
                    var result = new ObjectResult(new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = badRequest.Message,
                        Detail = badRequest.Message,
                        Instance = context.HttpContext.Request.Path
                    });

                    context.Result = result;
                    break;
            }
        }
    }
}
