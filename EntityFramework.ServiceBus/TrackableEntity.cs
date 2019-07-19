using System;
using EntityFramework.Triggers;

namespace EntityFramework.ServiceBus
{
    public abstract class TrackableEntity
    {
        public DateTime Inserted { get; private set; }
        public DateTime Updated { get; private set; }

        static TrackableEntity()
        {
            Triggers<TrackableEntity>.Inserting += entry => entry.Entity.Inserted = entry.Entity.Updated = DateTime.UtcNow;
            Triggers<TrackableEntity>.Updating += entry => entry.Entity.Updated = DateTime.UtcNow;
        }
    }
}