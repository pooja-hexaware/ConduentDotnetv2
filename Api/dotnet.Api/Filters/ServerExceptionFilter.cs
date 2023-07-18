using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using dotnet.Common.Exceptions;

namespace dotnet.Api.Filters
{
public class ServerExceptionFilter : IExceptionFilter
{
        private readonly ILogger<ServerExceptionFilter> _logger;

        public ServerExceptionFilter(ILogger<ServerExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ServerException serverException)
            {
                string errorMessage = serverException.Message;
                context.Result = new ObjectResult(errorMessage)
                { 
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                context.ExceptionHandled = true;
                _logger.LogError($"Error: " + $"Number: {serverException.SqlError.Number} " +
                                $"State: {serverException.SqlError.State} " +
                                $"Message: {serverException.SqlError.Message}");
            }
        }

    }
}
