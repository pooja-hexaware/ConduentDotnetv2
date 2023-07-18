using System;
using System.Data.SqlClient;

namespace dotnet.Common.Exceptions
{
    public class AppException : Exception
    {
        public SqlError SqlError { get; }
        public AppException(SqlError sqlError) : base(sqlError.Message) {
            SqlError = sqlError;
        }
    }
}
