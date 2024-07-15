using System.Data.SqlClient;
using System.Data;

namespace UserMicroservices.Respository.DB
{
    public class DbConnection
    {
        public class SqlDataAccess
        {
            private readonly IConfiguration _configuration;

            public SqlDataAccess(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            private IDbConnection GetConnection()
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                return new SqlConnection(connectionString);
            }

            public IDbConnection GetOpenConnection()
            {
                IDbConnection connection = GetConnection();
                connection.Open();
                return connection;
            }
        }
    }
}
