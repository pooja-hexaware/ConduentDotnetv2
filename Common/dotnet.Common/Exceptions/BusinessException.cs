using System;
using System.Data.SqlClient;

namespace dotnet.Common.Exceptions
{
    public class BusinessException : Exception
    {
        public SqlError SqlError { get; }

        public BusinessException(SqlError sqlError) : base(sqlError.Message) { 
            SqlError = sqlError;
        }

    }
}