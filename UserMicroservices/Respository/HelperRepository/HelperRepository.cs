using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using UserMicroservices.Dependencies;
using UserMicroservices.Interface;
using static UserMicroservices.Respository.DB.DbConnection;

namespace UserMicroservices.Repository.HelperRepository
{
    public class HelperRepository : IHelpers
    {
        private readonly Fast2SmsApi _fast2SmsApi;
        private readonly SqlDataAccess _sqlDataAccess;
        private const int OtpLength = 6; 

        public HelperRepository(Fast2SmsApi fast2SmsApi, SqlDataAccess sqlDataAccess)
        {
            _fast2SmsApi = fast2SmsApi;
            _sqlDataAccess = sqlDataAccess;
        }

        public bool GenerateAndSendOTP(string mobile,int userId)
        {
            try
            {
                DateTime expTime = DateTime.Now.AddMinutes(30);
                string otp = GenerateOtp(OtpLength);
                using (var connection = _sqlDataAccess.GetOpenConnection())
                {
                    using (var cmd = (SqlCommand)connection.CreateCommand())
                    {
                        cmd.CommandText = "usp_storeOTPs";
                        cmd.CommandType=CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@OTPVAlue", otp);
                        cmd.Parameters.AddWithValue("@ExpirationDateTime", expTime);
                        cmd.ExecuteNonQuery();
                    }
                }
                string response = _fast2SmsApi.SendSmsAsync(otp, mobile).Result;

                if (response.Contains("Error") || response.Contains("Exception"))
                {
                    Console.WriteLine($"Failed to send OTP: {response}");
                    return false;
                }

                Console.WriteLine("OTP sent successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }

        private string GenerateOtp(int length)
        {
            const string chars = "0123456789";
            var random = new Random();
            var otp = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                otp.Append(chars[random.Next(chars.Length)]);
            }

            return otp.ToString();
        }
    }
}
