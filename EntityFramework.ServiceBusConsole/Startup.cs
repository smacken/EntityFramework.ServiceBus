using System;
using System.IO;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.ServiceBus;
using EntityFramework.ServiceBus;

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

            string serviceBusConnectionString = Configuration.GetValue<string>("ConnectionStrings:AzureServiceBus");
            services.AddScoped<ITopicClient>(provider => new TopicClient(serviceBusConnectionString, nameof(ServiceDataContext)));

            string connectionString = Configuration.GetValue<string>("ConnectionStrings:Sqlite");
            var contextOptions = new DbContextOptionsBuilder<ServiceBusContext>()
                .UseSqlite(connectionString)
                .Options;
            services.AddSingleton(contextOptions);
            services.AddSingleton<ServiceBusConfiguration>(provider => Configuration.GetSection("serviceBus").Get<ServiceBusConfiguration>());
            services.AddScoped<ServiceDataContext>(provider => 
                new ServiceDataContext(provider.GetService<DbContextOptions<ServiceBusContext>>(), provider.GetService<ITopicClient>()));
            //services.AddDbContext<ServiceDataContext>(options => options.UseSqlite(connectionString));
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