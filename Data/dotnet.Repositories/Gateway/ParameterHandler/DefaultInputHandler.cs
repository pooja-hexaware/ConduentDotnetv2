using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotnet.Common.Session;

namespace dotnet.Repositories.Gateway.ParameterHandler
{
    public class DefaultInputHandler : IInputParameterHandler
    {
        private ISessionContext sessionContext;
        public DefaultInputHandler(ISessionContext _sessionContext)
        {
            sessionContext = _sessionContext;
        }
        public dynamic constructInputParameters(object parameters)
        {
            dynamic inputObject = new ExpandoObject();
            foreach (var property in parameters.GetType().GetProperties())
            {
                ((IDictionary<string, object>)inputObject)[property.Name] = property.GetValue(parameters);
            }
            inputObject.sessionId = sessionContext.sessionId;
            return inputObject;
        }
    }
}
