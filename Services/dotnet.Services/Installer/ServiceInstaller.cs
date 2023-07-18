using dotnet.Repositories.Installer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace dotnet.Services.Installer
{
    public class ServiceInstaller
    {
        public IConfiguration Configuration { get; }
        private readonly IServiceCollection _service;
        public ServiceInstaller(IServiceCollection service, IConfiguration iconfig)
        {
            Configuration = iconfig;
            _service = service;
        }

        public void Install()
        {
            _service.Scan(scan => scan
                                    .FromAssemblyOf<ServiceInstaller>()
                                    .AddClasses()
                                    .AsImplementedInterfaces()
                                    .WithScopedLifetime());
            var dataInstaller = new DataInstaller(_service, Configuration);
            dataInstaller.Install();
        }
    }
}
