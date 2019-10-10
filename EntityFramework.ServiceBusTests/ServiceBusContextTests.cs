using System;
using System.Threading.Tasks;
using EntityFramework.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Moq;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace EntityFramework.ServiceBusTests
{
    public class ServiceBusContextTests
    {
        [Fact]
        public void Ctor_CreatesNewContext()
        {
            var topicClient = new Mock<ITopicClient>();
            var options = SqliteInMemory.CreateOptions<ServiceBusContext>();
            using var context = new ServiceBusContext(options,topicClient.Object);
            context.ShouldNotBeNull();
        }

        [Fact]
        public async Task Context_SendsToTopic()
        {
            var topicClient = new Mock<ITopicClient>();
            topicClient.Setup(x => x.SendAsync(It.IsAny<Message>())).Returns(Task.CompletedTask).Verifiable();

            var options = SqliteInMemory.CreateOptions<ServiceBusContext>();
            await using var context = new TestContext(options, topicClient.Object);
            context.CreateEmptyViaWipe();
            await context.Customers.AddAsync(new Customer {FirstName = "Ted", LastName = "Bundy"});
            await context.SaveChangesAsync();

            topicClient.VerifyAll();
        }
    }
}
