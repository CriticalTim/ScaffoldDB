namespace ScaffoldDB.Data
{
    public class SchemaTableInfo
    {
        public string? SchemaName { get; set; }
        public List<string?> TableNames { get; set; } = new();
    }
}
