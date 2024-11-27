using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ScaffoldDB.Data
{
    public class DynamicContextInspector
    {
        public List<SchemaTableInfo> ExtractSchemaTableInfo(DbContext context)
        {
            try
            {
                var schemaTableInfo = context.Model.GetEntityTypes()
                .Select(entityType =>
                {
                    var schema = entityType.GetSchema() ?? "dbo"; // Default schema is dbo
                    var tableName = entityType.GetTableName();
                    return new { Schema = schema, Table = tableName };
                })
                .GroupBy(info => info.Schema)
                .Select(group => new SchemaTableInfo
                {
                    SchemaName = group.Key,
                    TableNames = group.Select(g => g.Table).OrderBy(t => t).ToList()
                })
                .OrderBy(info => info.SchemaName) // Order schemas alphabetically
                .ToList();

                return schemaTableInfo;
            }
            catch (Exception ex) 
            {
                int i = 0;
                return new List<SchemaTableInfo>();
            }
            
        }

        
            public async Task<TableData> ExtractTableData(DbContext context, string tableName, int offset, int limit)
            {
                try
                {
                    // Find the entity type for the specified table name
                    var entityType = context.Model.GetEntityTypes()
                        .FirstOrDefault(e => e.GetTableName().Equals(tableName, StringComparison.OrdinalIgnoreCase));

                    if (entityType == null)
                        throw new ArgumentException($"Table '{tableName}' does not exist in the current DbContext.");

                    // Get table name and CLR type
                    var clrType = entityType.ClrType;
                    if (clrType == null)
                        throw new Exception($"No CLR type found for table '{tableName}'.");


                // Use reflection to retrieve the DbSet dynamically
                var dbSetMethod = context.GetType()
                                .GetMethods()
                                .FirstOrDefault(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0);

                if (dbSetMethod == null)
                    throw new Exception("Unable to find the generic Set method.");

                var dbSet = dbSetMethod.MakeGenericMethod(clrType).Invoke(context, null);

                if (dbSet == null)
                    throw new Exception($"Unable to retrieve DbSet for table '{tableName}'.");

                // Use reflection to enumerate data
                var data = await ((IQueryable)dbSet).Cast<object>()
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();


                


                // Get property (column) names
                var properties = clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var columnNames = properties.Select(p => p.Name).ToList();

                // Extract rows of data
                var rows = data.Select(row =>
                    {
                        return properties.Select(p => p.GetValue(row)).ToList();
                    }).ToList();

                // Create and return table data
                return new TableData
                {
                        TableName = tableName,
                        ColumnNames = columnNames,
                        Rows = rows
                        
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while extracting data for table '{tableName}': {ex.Message}");
                    return null;
                }
            }

        public async Task<TableData> ExtractTableData(DbContext context, string tableName)
        {
            try
            {
                int offset = 0;
                int limit = 100;

                // Find the entity type for the specified table name
                var entityType = context.Model.GetEntityTypes()
                    .FirstOrDefault(e => e.GetTableName().Equals(tableName, StringComparison.OrdinalIgnoreCase));

                if (entityType == null)
                    throw new ArgumentException($"Table '{tableName}' does not exist in the current DbContext.");

                // Get table name and CLR type
                var clrType = entityType.ClrType;
                if (clrType == null)
                    throw new Exception($"No CLR type found for table '{tableName}'.");


                // Use reflection to retrieve the DbSet dynamically
                var dbSetMethod = context.GetType()
                                .GetMethods()
                                .FirstOrDefault(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0);

                if (dbSetMethod == null)
                    throw new Exception("Unable to find the generic Set method.");

                var dbSet = dbSetMethod.MakeGenericMethod(clrType).Invoke(context, null);

                if (dbSet == null)
                    throw new Exception($"Unable to retrieve DbSet for table '{tableName}'.");

                // Use reflection to enumerate data
                var data = await ((IQueryable)dbSet).Cast<object>()
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();


                // Get the total count of rows
                var totalCount = await ((IQueryable)dbSet).Cast<object>().CountAsync();


                // Get property (column) names
                var properties = clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var columnNames = properties.Select(p => p.Name).ToList();

                var columnDataTypes = properties.Select(p => p.PropertyType.Name).ToList();

                // Extract rows of data
                var rows = data.Select(row =>
                {
                    return properties.Select(p => p.GetValue(row)).ToList();
                }).ToList();

                // Create and return table data
                return new TableData
                {
                    TableName = tableName,
                    ColumnNames = columnNames,
                    Rows = rows,
                    RowCount = totalCount,
                    ColumnDataTypes = columnDataTypes
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while extracting data for table '{tableName}': {ex.Message}");
                return null;
            }
        }


    }
}
