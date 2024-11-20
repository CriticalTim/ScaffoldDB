using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ScaffoldDB.Data
{
    public static class DynamicContextExtensions
    {
        public static IQueryable Query(this DbContext context, string entityName) =>
        context.Query(entityName, context.Model.FindEntityType(entityName).ClrType);

        static readonly MethodInfo SetMethod =
            typeof(DbContext).GetMethod(nameof(DbContext.Set), 1, new[] { typeof(string) }) ??
            throw new Exception($"Type not found: DbContext.Set");

        public static IQueryable Query(this DbContext context, string entityName, Type entityType) =>
            (IQueryable)SetMethod.MakeGenericMethod(entityType)?.Invoke(context, new[] { entityName }) ??
            throw new Exception($"Type not found: {entityType.FullName}");
    }
}
