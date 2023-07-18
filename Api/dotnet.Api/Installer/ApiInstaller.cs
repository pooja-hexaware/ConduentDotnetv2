using dotnet.Api.Filters;
using dotnet.ServiceInterfaces;
using dotnet.Services.Installer;
using dotnet.Repositories.Gateway.SQLGatewayOptions;
using dotnet.Repositories.Gateway;
using dotnet.Common.Session;

namespace dotnet.Api.Installer
{
    public class ApiInstaller
    {
        private readonly IServiceCollection _service;
        public IConfiguration Configuration { get; }
        private readonly IServiceInstaller _serviceInstaller;

        public ApiInstaller(IServiceCollection service, IConfiguration configuration)
        {
            _service = service;
            Configuration = configuration;
        }

        public void Install()
        {
            _service.AddControllers(options =>
                {
                options.Filters.Add<HttpResponseExceptionFilter>();
                options.Filters.Add<ApplicationExceptionFilter>();
                options.Filters.Add<BusinessExceptionFilter>();
                options.Filters.Add<ServerExceptionFilter>();
                options.Filters.Add<SessionContextInitializer>();
                });

            string version = Configuration.GetValue("BuildVersion", "0.0.0.0");
            string branch = Configuration.GetValue("BuildBranch", "local");
            _service.AddScoped<ISessionContext, SessionContext>();
            var serviceInstaller = new ServiceInstaller(_service, Configuration);
            serviceInstaller.Install();
        }
    }
}
