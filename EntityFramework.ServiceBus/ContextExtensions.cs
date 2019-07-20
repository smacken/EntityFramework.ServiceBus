using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace EntityFramework.ServiceBus{
    public static class ContextExtensions{
        public static DbContext GetDbContext<T>(this DbSet<T> dbSet) where T:class{
            var infrastructure = dbSet as IInfrastructure<IServiceProvider>;
            var serviceProvider = infrastructure.Instance;
            var currentDbContext = serviceProvider.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
            return currentDbContext.Context;
        }
        public static string GetTableName<T>(this DbSet<T> dbSet) where T:class{
            var dbContext = dbSet.GetDbContext();
            var model = dbContext.Model;
            var entityTypes = model.GetEntityTypes();
            var entityType = entityTypes.First(t => t.ClrType == typeof(T));
            var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
            var tableName = tableNameAnnotation.Value.ToString();
            return tableName;
        }
    }
}