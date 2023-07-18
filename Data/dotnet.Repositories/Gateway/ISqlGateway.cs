using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;
using dotnet.Entities.Common;

namespace dotnet.Repositories.Gateway
{
  public interface ISqlGateway
  {
    Task<ListResponse<T>> ExecStoredProcReturnList<T>(string storedProcedure, object parameters);
    Task<T> ExecStoredProc<T>(string storedProcedure, object parameters);
    Task ExecStoredProc(string storedProcedure, object parameters);
    Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object args);
  }
}