namespace ScaffoldDB.Data
{
    public class TableData
    {
        public string TableName { get; set; }
        public List<string> ColumnNames { get; set; } = new();
        public List<List<object>> Rows { get; set; } = new();
    }
}
