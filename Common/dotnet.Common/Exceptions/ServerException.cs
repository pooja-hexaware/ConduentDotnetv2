using System;
using System.Data.SqlClient;

namespace dotnet.Common.Exceptions
{
    public class ServerException : Exception
    {
        public SqlError SqlError { get; }
        public ServerException(SqlError sqlError) : base(sqlError.Message) {
            SqlError = sqlError;
        }
    }
}
