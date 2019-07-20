using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.ServiceBusConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            IServiceCollection services = new ServiceCollection();
            Startup bootstrap = new Startup();
            bootstrap.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var config = serviceProvider.GetService<IConfigurationRoot>();
            string connectionString = config.GetValue<string>("ConnectionStrings:Sqlite");
            using (var db = new ServiceDataContext(connectionString))
            {
                await db.Customers.AddAsync(new Customer {FirstName = "Roger", LastName = "Gordon"});
            }
        }
    }
}
