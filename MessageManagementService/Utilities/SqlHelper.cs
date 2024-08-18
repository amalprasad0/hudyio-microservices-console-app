using System;
using System.Data;
using System.Data.SqlClient;
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

        private IDbConnection GetOpenConnection()
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public void ExecuteStoredProcedure(string storedProcedureName, Action<SqlCommand> parameterMapper, Action<SqlCommand> resultMapper)
        {
            using (var connection = GetOpenConnection())
            {
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    parameterMapper?.Invoke(cmd);
                    cmd.ExecuteNonQuery();
                    resultMapper?.Invoke(cmd);
                }
            }
        }

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
    }
}
