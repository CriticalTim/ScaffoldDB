namespace ScaffoldDB.Data
{
    public class SchemaSchema
    {
        public string SchemaName { get; set; }
        public List<TableSchema> Tables { get; set; }
    }
}
