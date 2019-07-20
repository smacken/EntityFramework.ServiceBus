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
        private IQueueClient _queueClient;
        public ServiceBusContext(string serviceBusConnectionString)
        {
            _queueClient = new QueueClient(serviceBusConnectionString, nameof(ServiceBusContext));
            Triggers<TrackableEntity, ServiceBusContext>.Inserted += e => 
            {
                _queueClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString()
                });
            };
            Triggers<TrackableEntity, ServiceBusContext>.Updated += e =>
            {
                
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
                _queueClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString()
                });
            };
            Triggers<T, ServiceBusContext>.Updated += e =>
            {
                _queueClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString()
                });
            };
            Triggers<T, ServiceBusContext>.Deleted += e =>
            {
                _queueClient.SendAsync(new Message
                {
                    Body = EntityAsPayload(e.Entity),
                    SessionId = Guid.NewGuid().ToString()
                });
            };
        }

        // public List<string> GetTables()
        // {
        //     var metadata = ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace;
            
        //     var tableNames = metadata.GetItems<EntityType>(DataSpace.SSpace)
        //         .Select(t => t.Name)
        //         .ToList();
        //     return tableNames;
        // }

        // protected override void Dispose(bool disposing)
        // {
        //     if (!_queueClient.IsClosedOrClosing) _queueClient.CloseAsync();
        //     base.Dispose(disposing);
        // }

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