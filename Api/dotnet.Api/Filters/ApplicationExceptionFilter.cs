using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Logging;
using dotnet.Common.Exceptions;

namespace dotnet.Api.Filters
{
public class ApplicationExceptionFilter : IExceptionFilter
{
        private readonly ILogger<ApplicationExceptionFilter> _logger;

        public ApplicationExceptionFilter(ILogger<ApplicationExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
    {
        if (context.Exception is AppException applicationException)
        {
            string errorMessage = applicationException.Message;
            context.Result = new ObjectResult(errorMessage)
            { 
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
            _logger.LogError($"Error: " + $"Number: {applicationException.SqlError.Number} " +
                                $"State: {applicationException.SqlError.State} " +
                                $"Message: {applicationException.SqlError.Message}");
            }
    }
}
}
