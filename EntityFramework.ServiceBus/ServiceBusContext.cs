using System;
using System.Collections.Generic;
using EntityFrameworkCore.Triggers;
using Microsoft.Azure.ServiceBus;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.ServiceBus
{
    public class ServiceBusContext : DbContextWithTriggers
    {
        public IQueueClient QueueClient { get; set; }

        public ServiceBusContext(DbContextOptions<ServiceBusContext> options, IQueueClient queueClient) :base(options)
        { 
            QueueClient = queueClient;
        }

        public ServiceBusContext(string serviceBusConnectionString)
        {
            Triggers<TrackableEntity, ServiceBusContext>.Inserted += e => 
            {
                QueueClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString()
                });
            };
            Triggers<TrackableEntity, ServiceBusContext>.Updated += e =>
            {
                QueueClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString()
                });
            };
            Triggers<TrackableEntity, ServiceBusContext>.Deleted += e =>
            {
                QueueClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString()
                });
            };
            
            // foreach (var table in GetTables())
            // {
            //     Type typeArgument = Type.GetType(table);
            //     Type closedType = typeof(Triggers<>).MakeGenericType(typeArgument);
            //     object trigger = Activator.CreateInstance(closedType);
                
            //     var insertedEvent = trigger.GetType().GetEvent("Inserted");
            //     //Delegate del = Delegate.CreateDelegate(
            //     //    insertedEvent.EventHandlerType, null, handler);
            //     //insertedEvent.AddEventHandler(trigger, () => );
                
            //     //Triggers.Triggers.Deleted += entry => 
            // }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public void ConfigureTriggers<T>() where T: TrackableEntity
        {
            Triggers<T, ServiceBusContext>.Inserted += e =>
            {
                QueueClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString()
                });
            };
            Triggers<T, ServiceBusContext>.Updated += e =>
            {
                QueueClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString()
                });
            };
            Triggers<T, ServiceBusContext>.Deleted += e =>
            {
                QueueClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString()
                });
            };
        }

        public List<string> GetTables()
        {
            var entities = this.Model.GetEntityTypes();
            return entities.Select(e => e.Name).ToList();
        }

        private byte[] EntityAsPayload(TrackableEntity entity)
        {
            var payload = JsonConvert.SerializeObject(entity, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            return Encoding.UTF8.GetBytes(payload);
        }

    }
}