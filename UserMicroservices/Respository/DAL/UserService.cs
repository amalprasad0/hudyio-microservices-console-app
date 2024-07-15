using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using UserMicroservices.Interface;
using UserMicroservices.Models;
using UserMicroservices.Respository.DB;
using static UserMicroservices.Respository.DB.DbConnection;

namespace UserMicroservices.Respository.DAL
{
    public class UserMember : IUser
    {
        private readonly SqlDataAccess _sqlDataAccess;
        private readonly IHelpers _helpers;

        public UserMember(SqlDataAccess sqlDataAccess, IHelpers helpers)
        {
            _sqlDataAccess = sqlDataAccess;
            _helpers = helpers;
        }

        public void CreateUser(CreateUser user)
        {
            try
            {
                string hashedPasskey = _helpers.HashPasskey(user.Passkey);

                using (var connection = _sqlDataAccess.GetOpenConnection())
                {
                    using (var cmd = (SqlCommand)connection.CreateCommand())
                    {
                        cmd.CommandText = "CREATEUSERCORE";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@name", user.Name);
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@phone", user.Phone);
                        cmd.Parameters.AddWithValue("@email", user.Email);
                        cmd.Parameters.AddWithValue("@passkey", hashedPasskey); // Store hashed passkey
                        cmd.Parameters.AddWithValue("@createdBy", user.CreatedBy);
                        if (user.ModifiedBy != null)
                        {
                            cmd.Parameters.AddWithValue("@modifiedBy", user.ModifiedBy);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@modifiedBy", DBNull.Value);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public bool loginUser(LoginUser login)
        {
            try
            {
                string hashedPasskey = _helpers.HashPasskey(login.Password);
                using (var connection = _sqlDataAccess.GetOpenConnection())
                {
                    using (var cmd = (SqlCommand)connection.CreateCommand())
                    {
                        cmd.Parameters.Clear();

                        if (string.IsNullOrEmpty(login.Email))
                        {
                            cmd.CommandText = "LOGINUSERCOREUSINGUSERNAME";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@username", login.Username);
                        }
                        else
                        {
                            cmd.CommandText = "LOGINUSERCOREUSINGEMAIL";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@useremail", login.Email);
                        }

                        cmd.Parameters.AddWithValue("@passkey", hashedPasskey);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader.GetBoolean(reader.GetOrdinal("UserExists"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return false;

        }
        public List<UserData> GetUserData(string? userEmail, string? userName)
        {
            try
            {
                List<UserData> userDataList = new List<UserData>();

                using (var connection = _sqlDataAccess.GetOpenConnection())
                {
                    using (var cmd = (SqlCommand)connection.CreateCommand())
                    {
                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            cmd.CommandText = "GETUSERDATACOREUSINGEMAIL";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@userEmail", userEmail);
                        }
                        else if (!string.IsNullOrEmpty(userName))
                        {
                            cmd.CommandText = "GETUSERDATACOREUSINGUSERNAME";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@userName", userName);
                        }
                        else
                        {
                            throw new ArgumentException("Either userEmail or userName must be provided.");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserData data = new UserData
                                {
                                    Name = reader["name"].ToString(),
                                    Username = reader["username"].ToString(),
                                    Phone = reader["phone"].ToString(),
                                    Email = reader["email"].ToString(),
                                    CreatedAt = reader["createdAt"] != DBNull.Value ? (DateTime)reader["createdAt"] : (DateTime?)null,
                                    ModifiedAt = reader["modifiedAt"] != DBNull.Value ? (DateTime)reader["modifiedAt"] : (DateTime?)null,
                                    CreatedBy = reader["createdBy"].ToString(),
                                    ModifiedBy = reader["modifiedBy"].ToString()
                                };

                                userDataList.Add(data);
                            }
                        }
                    }
                }

                return userDataList;
            }
            catch (Exception ex)
            {
                throw new Exception("Internal Server Error: " + ex.Message);
            }
        }
        public bool UpdatePassword(string? userName, string? userEmail, string userPassword)
        {
            try
            {
                string hashedPassword = _helpers.HashPasskey(userPassword);

                using (var connection = _sqlDataAccess.GetOpenConnection())
                {
                    using (var cmd = (SqlCommand)connection.CreateCommand())
                    {
                        cmd.CommandText = userName != null ? "UPDATEUSERCOREPASSWORDBYUSERNAME" : "UPDATEUSERCOREPASSWORDBYUSEREMAIL";
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (userName != null)
                        {
                            cmd.Parameters.AddWithValue("@userName", userName);
                        }
                        else if (userEmail != null)
                        {
                            cmd.Parameters.AddWithValue("@userEmail", userEmail);
                        }
                        cmd.Parameters.AddWithValue("@userPassword", hashedPassword);

                        SqlParameter updatedParam = new SqlParameter("@Updated", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(updatedParam);

                        cmd.ExecuteNonQuery();

                        return (bool)updatedParam.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Internal Server Error: " + ex.Message);
            }
        }


    }
}
