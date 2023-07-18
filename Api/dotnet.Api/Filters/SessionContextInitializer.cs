using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using dotnet.Common.Session;

namespace dotnet.Api.Filters
{
    public class SessionContextInitializer : IActionFilter
    {
        private ISessionContext _sessionContext;
        

        public SessionContextInitializer(ISessionContext sessionContext)
        {
            _sessionContext = sessionContext;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            StringValues sessionId;
            bool sessionIdPresent = context.HttpContext.Request.Headers.TryGetValue("sessionid", out sessionId);
            _sessionContext.sessionId = sessionIdPresent ? Convert.ToInt32(sessionId[0]) : 999;
        }
    }
}