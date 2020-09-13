using ippt.dem.mesh.repository;
using ippt.dem.mesh.system;
using ippt.dem.mesh.system.parser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ippt.dem.mesh.app
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            
            string path = @"1_50.inp";
            
            var services = new ServiceCollection();
            ConfigureServices(services);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var app = serviceProvider.GetService<MeshApp>();
                app.Run(path);
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<MeshApp>();

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddNLog("nlog.config");
            });
            services.AddSingleton<ICoreTextFileRead, CoreTextFileReadImpl>();
            services.AddSingleton<DataRepository, InMemoryDataRepository>();
            services.AddSingleton<IAbaqusParser, ConcreteAbaqusParser>();
        }
    }
}