using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet.Repositories.Gateway.ParameterHandler
{
    public interface IInputParameterHandler
    {
       public object constructInputParameters(object parameters);
    }
}
