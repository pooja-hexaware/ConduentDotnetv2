

namespace dotnet.Repositories.Gateway.SQLGatewayOptions
{
    public class SqlGatewayOptions : ISqlGatewayOptions
    {
        public  string connectionString { get; set; }

        public SqlGatewayOptions(string ConnectionString)
        {
            connectionString = ConnectionString;
        }
    }
}
