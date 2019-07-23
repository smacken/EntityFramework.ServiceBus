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
        public ITopicClient TopicClient { get; set; }

        public ServiceBusContext(DbContextOptions<ServiceBusContext> options, ITopicClient topicClient) :base(options)
        { 
            TopicClient = topicClient;
        }

        public ServiceBusContext(string serviceBusConnectionString)
        {
            Triggers<TrackableEntity, ServiceBusContext>.Inserted += e => 
            {
                TopicClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString(),
                    ContentType = "application/json",
                    Label = e.Context.Set<TrackableEntity>().GetTableName(),
                });
            };
            Triggers<TrackableEntity, ServiceBusContext>.Updated += e =>
            {
                TopicClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString(),
                    Label = e.Context.Set<TrackableEntity>().GetTableName(),
                });
            };
            Triggers<TrackableEntity, ServiceBusContext>.Deleted += e =>
            {
                TopicClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString(),
                    Label = e.Context.Set<TrackableEntity>().GetTableName(),
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
                TopicClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString(),
                    ContentType = "application/json",
                    Label = Set<T>().GetTableName()
                });
            };
            Triggers<T, ServiceBusContext>.Updated += e =>
            {
                TopicClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString(),
                    ContentType = "application/json",
                    Label = Set<T>().GetTableName()
                });
            };
            Triggers<T, ServiceBusContext>.Deleted += e =>
            {
                TopicClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString(),
                    ContentType = "application/json",
                    Label = Set<T>().GetTableName()
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