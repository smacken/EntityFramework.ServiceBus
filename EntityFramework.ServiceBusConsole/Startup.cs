using System;
using System.IO;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.ServiceBusConsole
{
    public class Startup
    {
        IConfigurationRoot Configuration { get; }
        public Startup()
        {
            Configuration = LoadAppSettings();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddLogging();
            services.AddSingleton<IConfigurationRoot>(Configuration);

            //example
            //services.AddSingleton<IMyService, MyService>();
            services.AddSingleton<IAppHost, AppHost>();
        }

        private static IConfigurationRoot LoadAppSettings()
        {
            try
            {
                var basePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "EntityFramework.ServiceBusConsole");
                var config = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", false, true)
                    .AddEnvironmentVariables()
                    .Build();
                
                return config;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
        }
    }
}