using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldDB.Services
{
    public class DynamicTableService
    {
        private readonly string _connectionString;

        public DynamicTableService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> InsertAsync(string tableName, Dictionary<string, object> data)
        {
            if (string.IsNullOrWhiteSpace(tableName) || data == null || data.Count == 0)
                throw new ArgumentException("Table name and data cannot be null or empty.");

            if (!IsValidIdentifier(tableName) || !data.Keys.All(IsValidIdentifier))
                throw new ArgumentException("Invalid table name or column names.");

            // Build the INSERT query dynamically
            var query = new StringBuilder();
            query.Append($"INSERT INTO {tableName} (");
            query.Append(string.Join(", ", data.Keys)); // Columns
            query.Append(") VALUES (");
            query.Append(string.Join(", ", data.Keys.Select((_, index) => $"@p{index}"))); // Parameters
            query.Append(");");

            // Create parameters for the values
            var parameters = data.Select((kvp, index) => new SqlParameter($"@p{index}", kvp.Value ?? DBNull.Value)).ToArray();

            // Execute the query
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query.ToString(), connection))
                {
                    command.Parameters.AddRange(parameters);
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> UpdateAsync(string tableName, Dictionary<string, object> data, Dictionary<string, object> conditions)
        {
            if (string.IsNullOrWhiteSpace(tableName) || data.Count == 0)
                throw new ArgumentException("Table name or data is invalid.");

            if (!IsValidIdentifier(tableName) || !data.Keys.All(IsValidIdentifier) || !conditions.Keys.All(IsValidIdentifier))
                throw new ArgumentException("Invalid table name or column names.");

            // Construct SQL dynamically
            var query = new StringBuilder();
            query.Append($"UPDATE {tableName} SET ");

            var parameters = new List<SqlParameter>();
            int index = 0;

            foreach (var kvp in data)
            {
                query.Append($"{kvp.Key} = @p{index}, ");
                parameters.Add(new SqlParameter($"@p{index}", kvp.Value ?? DBNull.Value));
                index++;
            }

            query.Length -= 2; // Remove trailing comma and space
            query.Append(" WHERE ");

            foreach (var kvp in conditions)
            {
                query.Append($"{kvp.Key} = @c{index} AND ");
                parameters.Add(new SqlParameter($"@c{index}", kvp.Value ?? DBNull.Value));
                index++;
            }

            query.Length -= 4; // Remove trailing " AND"

            // Execute the query
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query.ToString(), connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<int> DeleteAsync(string tableName, int id)
        {
            

            // Construct SQL dynamically
            var query = new StringBuilder();
            query.Append($"DELETE FROM {tableName} WHERE id = {id}");

            

            // Execute the query
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query.ToString(), connection))
                {
                    
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Validates table and column identifiers.
        /// </summary>
        private bool IsValidIdentifier(string identifier)
        {
            return !string.IsNullOrWhiteSpace(identifier) &&
                   System.Text.RegularExpressions.Regex.IsMatch(identifier, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }
    }
}
