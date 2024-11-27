namespace ScaffoldDB.Data
{
    public class TableData
    {
        public string TableName { get; set; }
        public List<string> ColumnNames { get; set; } = new();
        public List<string> ColumnDataTypes { get; set; }
        public List<List<object>> Rows { get; set; } = new();

        public int RowCount { get; set; } = 0;
    }
}
