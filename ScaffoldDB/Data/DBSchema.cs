namespace ScaffoldDB.Data
{
    public class DBSchema
    {
        public string DBName { get; set; }
        public List<SchemaSchema> Schemas { get; set; }
    }
}
