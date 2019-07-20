﻿using EntityFramework.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.ServiceBusConsole
{
    public class ServiceDataContext : ServiceBusContext
    {
        private string _connectionString = string.Empty;
        public virtual DbSet<Customer> Customers { get; set; }

        public ServiceDataContext(string serviceBusConnectionString, string databaseConnectionString) : base(serviceBusConnectionString)
        {
            _connectionString = databaseConnectionString;
            ConfigureTriggers<Customer>();
        }

        public ServiceDataContext(DbContextOptions<ServiceBusContext> options, IQueueClient queueClient) : base(options, queueClient)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
                optionsBuilder.UseSqlite(_connectionString);
        }
    }
}