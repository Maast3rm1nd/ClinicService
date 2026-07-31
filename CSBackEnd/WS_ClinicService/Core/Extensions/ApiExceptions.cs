using Microsoft.AspNetCore.Mvc.Filters;

namespace WS_ClinicService.Core.Extensions
{
    public class ApiExceptions : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            { 
                
            }
        }
    }
}
