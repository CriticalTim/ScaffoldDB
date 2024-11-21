using Microsoft.EntityFrameworkCore;

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
    }
}
