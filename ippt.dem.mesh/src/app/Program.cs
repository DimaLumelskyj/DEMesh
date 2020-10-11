using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.entities.finite.element;
using ippt.dem.mesh.entities.nodes;
using ippt.dem.mesh.repository;
using ippt.dem.mesh.system;
using ippt.dem.mesh.system.parser;
using ippt.dem.mesh.system.write;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ippt.dem.mesh.app
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            
            string path = @"C:\Users\dimal\RiderProjects\DEMesh\ippt.dem.mesh\bin\Debug\netcoreapp3.1\1_50.inp";
            
            var services = new ServiceCollection();
            ConfigureServices(services);

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var app = serviceProvider.GetService<MeshApp>();
                app?.Run(path);
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
            services.AddSingleton<IDataRepository, InMemoryDataRepository>();
            services.AddSingleton<IAbaqusParser, ConcreteAbaqusParser>();
            services.AddSingleton<NodeCreator,ConcreteNodeCreator>();
            services.AddSingleton<ElementCreator,ConcreteElementCreator>();
            services.AddSingleton<DiscreteElementCreator, ConcreteDiscreteElementCreator>();
            services.AddSingleton<IWriteOutputResults, WriteOutputResults>();
        }
    }
}