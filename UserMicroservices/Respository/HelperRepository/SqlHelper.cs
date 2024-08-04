using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UserMicroservices.Models;
using static UserMicroservices.Respository.DB.DbConnection;

namespace UserMicroservices.Respository
{
    public class SqlHelper
    {
        private readonly SqlDataAccess _sqlDataAccess;

        public SqlHelper(SqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public void ExecuteStoredProcedure(string storedProcedureName, Action<SqlCommand> parameterMapper, Action<SqlCommand> resultMapper)
        {
            using (var connection = _sqlDataAccess.GetOpenConnection())
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
            using (var connection = _sqlDataAccess.GetOpenConnection())
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