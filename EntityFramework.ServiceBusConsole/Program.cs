using System;
using System.Threading.Tasks;
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
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IAppHost, AppHost>()
                .BuildServiceProvider();

            string connectionString = string.Empty;
            using (var db = new ServiceDataContext(connectionString))
            {
                await db.Customers.AddAsync(new Customer {FirstName = "Roger", LastName = "Gordon"});
            }
        }
    }

    public interface IAppHost
    {
        void Run();
    }

    public class AppHost : IAppHost
    {
        
        public AppHost()
        {
            
        }

        public void Run()
        {
        }
    }
}
