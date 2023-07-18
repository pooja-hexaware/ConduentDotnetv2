using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Data;
using System;
using dotnet.Repositories.Gateway.SQLGatewayOptions;
using dotnet.Repositories.Gateway.ParameterHandler;
using dotnet.Entities.Common;
using dotnet.Common.Exceptions;
using dotnet.Common.Session;

namespace dotnet.Repositories.Gateway
{
public class SqlGateway: ISqlGateway
{
        private IInputParameterHandler  inputParameterHandler;
        private ISqlGatewayOptions sqlGatewayOptions;

         public SqlGateway(ISqlGatewayOptions _sqlGatewayOptions, ISessionContext sessionContext)
        {
            sqlGatewayOptions = _sqlGatewayOptions;
            inputParameterHandler = new DefaultInputHandler(sessionContext);

        }

        private void handleErrors(SqlError sqlError, int status)
        {
            if (status == 1)
                throw new BusinessException(sqlError);
            else if (status == 2)
                throw new ServerException(sqlError);
            else if (status == 3)
                throw new AppException(sqlError);
        }

        public async Task ExecStoredProc(string storedProcedure, object parameters)
        {
            SqlError sqlError = null;
            var inputObject = inputParameterHandler.constructInputParameters(parameters);
            using (var connection = new SqlConnection(sqlGatewayOptions.connectionString))
            {
                connection.InfoMessage += (sender, e) => sqlError = e.Errors[0];
                var result = await connection.QueryMultipleAsync(
                    storedProcedure,
                    (object)inputObject,
                    commandType: CommandType.StoredProcedure
                );
                var status = await result.ReadSingleAsync<int>();
                handleErrors(sqlError, status);
            }
        }
        public async Task<T> ExecStoredProc<T>(string storedProcedure, object parameters)
        {
            T response = default(T);
            SqlError sqlError = null;
            dynamic inputObject = inputParameterHandler.constructInputParameters(parameters);
            using (var connection = new SqlConnection(sqlGatewayOptions.connectionString))
            {
                connection.InfoMessage += (sender, e) => sqlError = e.Errors[0];

                var result = await connection.QueryMultipleAsync(
                    storedProcedure,
                    (object)inputObject,
                    commandType: CommandType.StoredProcedure
                );
                var status = await result.ReadSingleAsync<int>();
                if (!result.IsConsumed)
                {
                    response = result.ReadAsync<T>().Result.First();
                }
                if (status == 0)
                    return response;
                else
                    handleErrors(sqlError, status);
            }
            return default(T);
        }


        public async Task<ListResponse<T>> ExecStoredProcReturnList<T>(string storedProcedure, object parameters)
        {
            ListResponse<T> response = new ListResponse<T>();
            int status = int.MaxValue;
            int totalCount = 0;
            SqlError sqlError = null;
            var inputObject = inputParameterHandler.constructInputParameters(parameters);

            using (var connection = new SqlConnection(sqlGatewayOptions.connectionString))
            {
                connection.InfoMessage += (sender, e) => sqlError = e.Errors[0];
                await connection.OpenAsync();
                var command = new SqlCommand(storedProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                foreach (var property in ((IDictionary<string, object>)inputObject))
                {
                    command.Parameters.AddWithValue(property.Key, property.Value);
                }
                var reader = await command.ExecuteReaderAsync();
                var columnDetails = new List<PropertyMetaData>();
                if (reader.HasRows)
                {
                  while (await reader.ReadAsync())
                    {
                        var schemaTab = reader.GetSchemaTable();
                        if (schemaTab.Rows.Cast<DataRow>().Any(row => row["ColumnName"].ToString() == "TotalCount")
)
                        {
                            totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                            response.TotalCount = totalCount;
                        }

                        status = reader.GetInt32(reader.GetOrdinal("status"));
                    }

                    await reader.NextResultAsync();
                    var resultSet = new List<T>();
                    var schemaTable = default(DataTable);

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            schemaTable = reader.GetSchemaTable();
                            var item = ParseRow<T>(reader);
                            resultSet.Add(item);
                        }

                        var result = resultSet.AsEnumerable();
                        response.Result = result;

                        if (schemaTable != null)
                        {
                            columnDetails = ExtractColumnDetails(schemaTable);
                        }

                        response.ResultMetaData = columnDetails;
                    }
                }
                if (status == 0)
                    return response;
                else
                    handleErrors(sqlError, status);
            }

            return default(ListResponse<T>);
        }

        private T ParseRow<T>(SqlDataReader reader)
        {
            var properties = typeof(T).GetProperties();
            var item = Activator.CreateInstance<T>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var property = properties.FirstOrDefault(p => p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                if (property != null && reader[columnName] != DBNull.Value)
                {
                    var value = Convert.ChangeType(reader[columnName], property.PropertyType);
                    property.SetValue(item, value);
                }
            }

            return item;
        }
        private List<PropertyMetaData> ExtractColumnDetails(DataTable schemaTable)
        {
            var columnDetails = new List<PropertyMetaData>();

            foreach (DataRow row in schemaTable.Rows)
            {
                var columnName = (string)row["ColumnName"];
                var dataType = ((Type)row["DataType"]).FullName;
                var isNullable = (bool)row["AllowDBNull"];

                var columnDetail = new PropertyMetaData
                {
                    columnName = columnName,
                    dataType = dataType,
                    isNull = isNullable
                };

                columnDetails.Add(columnDetail);
            }

            return columnDetails;
        }

        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object args)
        {
            using var connection = new SqlConnection(sqlGatewayOptions.connectionString);
            var command = connection.CreateCommand();

            var res = await connection.QueryAsync<T>(sql, args);
            return res;
        }


}
}