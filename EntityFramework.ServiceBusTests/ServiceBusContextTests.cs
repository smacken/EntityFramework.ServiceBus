using System;
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
    }
}
