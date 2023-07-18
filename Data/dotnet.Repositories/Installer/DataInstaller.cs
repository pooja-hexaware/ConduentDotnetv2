using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using dotnet.Repositories.Gateway.SQLGatewayOptions;

namespace dotnet.Repositories.Installer
{
    public class DataInstaller
    {
        public IConfiguration Configuration { get; }

        private readonly IServiceCollection _service;
        public DataInstaller(IServiceCollection service, IConfiguration iconfig)
        {
            _service = service;
            Configuration = iconfig;
        }

        public void Install()
        {
            _service.Scan(scan => scan
                                    .FromAssemblyOf<DataInstaller>()
                                    .AddClasses()
                                    .AsImplementedInterfaces()
                                    .WithScopedLifetime());

            _service.Remove(_service.First(o => o.ImplementationType == typeof(SqlGatewayOptions)));

            _service.AddScoped<ISqlGatewayOptions>(provider =>
            {
                var configuration = provider.GetService<IConfiguration>();
                var connectionString = Configuration.GetConnectionString("defaultConnection");
                return new SqlGatewayOptions(connectionString);
            });



        }
    }
}
