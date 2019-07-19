using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Transactions;
using EntityFramework.ServiceBus;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.ServiceBusConsole
{
    public class ServiceDataContext : ServiceBusContext
    {
        public ServiceDataContext(string serviceBusConnectionString) : base(serviceBusConnectionString)
        {
            ConfigureTriggers<Customer>();
            
        }

        public virtual Microsoft.EntityFrameworkCore.DbSet<Customer> Customers { get; set; }

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./blog.db");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Customer>()
                .HasRequired(e => e.LastName);
        }

    }
}