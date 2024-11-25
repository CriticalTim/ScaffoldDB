namespace ScaffoldDB.Services
{
    public class DynamicTableServiceFactory : IDynamicTableServiceFactory
    {
        public DynamicTableService Create(string connectionString)
        {
            return new DynamicTableService(connectionString);
        }
    }
}
