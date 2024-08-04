using Microsoft.AspNetCore.Mvc.Routing;
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

        public Response<int> StoreUserAndSendOTP(CreateUser user)
        {
            var response = new Response<int>();

            try
            {
                var sqlHelper = new SqlHelper(_sqlDataAccess);

                int newUserId = 0;
                sqlHelper.ExecuteStoredProcedure(
                    "usp_CreateUserRecord",
                    cmd =>
                    {
                        cmd.Parameters.AddWithValue("@Name", user.Name);
                        cmd.Parameters.AddWithValue("@mobile_number", user.Phone);
                        cmd.Parameters.AddWithValue("@email", user.Email);
                        cmd.Parameters.AddWithValue("@isDisabled", true);

                        var userIdParam = new SqlParameter("@UserId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(userIdParam);
                    },
                    cmd =>
                    {
                        newUserId = (int)cmd.Parameters["@UserId"].Value;
                    });

                bool isOtpDispatched = _helpers.GenerateAndSendOTP(user.Phone, newUserId);
                if (isOtpDispatched)
                {
                    response.Success = true;
                    response.Data = newUserId;
                }
                else
                {
                    response.Success = false;
                    response.Data = 1;
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
        public Response<bool> CheckOtpandRegisterUser(LoginParams loginParams)
        {
            var response = new Response<bool>();

            try
            {
                var sqlHelper = new SqlHelper(_sqlDataAccess);

                sqlHelper.ExecuteStoredProcedureWithResult(
                    "usp_CheckOTPandActivateUser",
                    cmd =>
                    {
                        cmd.Parameters.AddWithValue("@userId", loginParams.userId);
                        cmd.Parameters.AddWithValue("@OtpNumber", loginParams.otp);
                    },
                    reader =>
                    {
                        if (reader.Read())
                        {
                            response.Data = reader.GetInt32(0) == 1;
                        }
                    });

                response.Success = true;
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



