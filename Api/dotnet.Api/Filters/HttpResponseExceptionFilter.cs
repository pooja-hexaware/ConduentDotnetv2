using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dotnet.Api.Filters
{
    public class HttpResponseExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<HttpResponseExceptionFilter> _logger;

        public HttpResponseExceptionFilter(ILogger<HttpResponseExceptionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred");

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.HttpContext.Response.ContentType = "application/json";

            var result = new ObjectResult("An error occurred")
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            await context.HttpContext.Response.WriteAsJsonAsync(result);
            context.ExceptionHandled = true;
        }
    }
}
