using EntityFramework.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.ServiceBusTests
{
    public class TestContext : ServiceBusContext
    {
        public TestContext(DbContextOptions<ServiceBusContext> options, ITopicClient topicClient) : base(options, topicClient)
        {
            ConfigureTriggers<Customer>();
        }

        public TestContext(string serviceBusConnectionString) : base(serviceBusConnectionString)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
    }
}