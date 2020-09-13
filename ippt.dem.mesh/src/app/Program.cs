using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ippt.dem.mesh.app
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var app = serviceProvider.GetService<MeshApp>();
                app.Run();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<MeshApp>();

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddNLog(".\\configuration\\nlog.config");
            });
        }
    }
}