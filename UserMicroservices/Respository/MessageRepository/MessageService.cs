using System.Data.SqlClient;
using System.Data;
using UserMicroservices.Interface;
using UserMicroservices.Models;
using static UserMicroservices.Respository.DB.DbConnection;

namespace UserMicroservices.Respository.MessageRepository
{
    public class MessageService:IMessage
    {
       
        private readonly SqlDataAccess _sqlDataAccess;

        public MessageService(SqlDataAccess sqlDataAccess) {
          
            _sqlDataAccess = sqlDataAccess;
        
        }
        public async Task<Response<bool>> StoreUserMessage(UserMessage userMessage)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                var sqlHelper=new SqlHelper(_sqlDataAccess);
                sqlHelper.ExecuteStoredProcedure("usp_Store_Message", cmd =>
                {
                    cmd.Parameters.AddWithValue("@CacheMessageId", userMessage.cachedMessageId);
                    cmd.Parameters.AddWithValue("@FromUserId", userMessage.fromUserId);
                    cmd.Parameters.AddWithValue("@ToUserId", userMessage.toUserId);
                    cmd.Parameters.AddWithValue("@MessageContent", userMessage.messageContent);
                    cmd.Parameters.AddWithValue("@HasFile", userMessage.hasFile);
                    cmd.Parameters.AddWithValue("@FileUrl", userMessage.fileUrl);
                    cmd.Parameters.AddWithValue("@FileType", userMessage.fileType);
                    var userIdParam = new SqlParameter("@Success", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(userIdParam);
                }, cmd =>
                {
                    response.Data = (bool)cmd.Parameters["@Success"].Value;
                }
                );
                response.Success = true;
            }
            catch (Exception ex) {
                response.Success = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
       
    }
}
