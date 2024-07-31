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

        public Response<bool> StoreUserAndSendOTP(CreateUser user)
        {
            var response = new Response<bool>();

            try
            {
                using (var connection = _sqlDataAccess.GetOpenConnection())
                {
                    using (var cmd = (SqlCommand)connection.CreateCommand())
                    {
                        cmd.CommandText = "usp_CreateUserRecord";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Name", user.Name);
                        cmd.Parameters.AddWithValue("@mobile_number", user.Phone);
                        cmd.Parameters.AddWithValue("@email", user.Email);
                        cmd.Parameters.AddWithValue("@isDisabled", false);
                        var userIdParam = new SqlParameter("@UserId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(userIdParam);
                        cmd.ExecuteNonQuery();
                        int newUserId = (int)userIdParam.Value;
                        bool isOtpDispatched = _helpers.GenerateAndSendOTP(user.Phone, newUserId);
                        response.Success = true;
                        response.Data = true; 
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.Success = false;
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = $"Error: {ex.Message}";
            }

            return response;
        }


    }
}



