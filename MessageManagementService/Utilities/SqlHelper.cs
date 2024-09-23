using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MessageManagementService.Utilities
{
    public class SqlHelper
    {
        private readonly string _connectionString;

        public SqlHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Synchronous method to open a connection
        private IDbConnection GetOpenConnection()
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        // Asynchronous method to open a connection
        private async Task<IDbConnection> GetOpenConnectionAsync()
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            await ((SqlConnection)connection).OpenAsync();
            return connection;
        }

        // Synchronous execution of a stored procedure without returning results
        public void ExecuteStoredProcedure(string storedProcedureName, Action<SqlCommand> parameterMapper, Action<SqlCommand> resultMapper)
        {
            using (var connection = GetOpenConnection())
            {
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Map parameters, if any
                    parameterMapper?.Invoke(cmd);

                    // Execute the stored procedure
                    cmd.ExecuteNonQuery();

                    // Map the result, if any
                    resultMapper?.Invoke(cmd);
                }
            }
        }

        // Asynchronous execution of a stored procedure without returning results
        public async Task ExecuteStoredProcedureAsync(string storedProcedureName, Action<SqlCommand> parameterMapper, Action<SqlCommand> resultMapper)
        {
            using (var connection = await GetOpenConnectionAsync())
            {
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    parameterMapper?.Invoke(cmd);
                    await cmd.ExecuteNonQueryAsync();
                    resultMapper?.Invoke(cmd);
                }
            }
        }

        // Synchronous execution of a stored procedure and returning results
        public void ExecuteStoredProcedureWithResult(string storedProcedureName, Action<SqlCommand> parameterMapper, Action<SqlDataReader> resultMapper)
        {
            using (var connection = GetOpenConnection())
            {
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    parameterMapper?.Invoke(cmd);

                    using (var reader = cmd.ExecuteReader())
                    {
                        resultMapper?.Invoke(reader);
                    }
                }
            }
        }

        // Asynchronous execution of a stored procedure and returning results
        public async Task ExecuteStoredProcedureWithResultAsync(string storedProcedureName, Action<SqlCommand> parameterMapper, Action<SqlDataReader> resultMapper)
        {
            using (var connection = await GetOpenConnectionAsync())
            {
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    parameterMapper?.Invoke(cmd);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        resultMapper?.Invoke(reader);
                    }
                }
            }
        }

        // Asynchronous execution with generic result mapping
        public async Task<List<T>> ExecuteStoredProcedureWithResultAsync<T>(string storedProcedureName, Action<SqlCommand> parameterMapper, Func<SqlDataReader, T> resultMapper)
        {
            List<T> results = new List<T>();

            using (var connection = await GetOpenConnectionAsync())
            {
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    parameterMapper?.Invoke(cmd);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(resultMapper(reader));
                        }
                    }
                }
            }

            return results;
        }
    }
}
