using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using dotnet.Common.Exceptions;

namespace dotnet.Api.Filters
{
public class BusinessExceptionFilter : IExceptionFilter
{
    private readonly ILogger<BusinessExceptionFilter> _logger;

        public BusinessExceptionFilter(ILogger<BusinessExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
    {
        if (context.Exception is BusinessException businessException) 
        {
                string errorMessage = businessException.Message;
                context.Result = new ObjectResult(errorMessage) 
            { 
                StatusCode = StatusCodes.Status400BadRequest
            };
            context.ExceptionHandled = true;
            _logger.LogError($"Error: " + $"Number: {businessException.SqlError.Number} " + 
                                $"State: {businessException.SqlError.State} " + 
                                $"Message: {businessException.SqlError.Message}");
                         }
            
        }
        
}
}
