using System;
using EntityFrameworkCore.Triggers;

namespace EntityFramework.ServiceBus
{
    public abstract class SoftDeletable : TrackableEntity
    {
        public virtual DateTime? Deleted { get; private set; }

        public Boolean IsSoftDeleted => Deleted != null;
        public void SoftDelete() => Deleted = DateTime.UtcNow;
        public void SoftRestore() => Deleted = null;

        static SoftDeletable()
        {
            Triggers<SoftDeletable>.Deleting += entry => {
                entry.Entity.SoftDelete();
                entry.Cancel = true; // Cancels the deletion, but will persist changes with the same effects as EntityState.Modified
            };
        }
    }
}