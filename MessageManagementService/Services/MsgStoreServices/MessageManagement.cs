using MessageManagementService.Interface;
using MessageManagementService.Utilities;
using MessageManagementService.Models;
using System.Data.SqlClient;
using System.Data;

namespace MessageManagementService.Services.MsgStoreServices
{
    public class MessageManagement : IMessageManagement
    {
        private readonly SqlHelper _sqlHelper;
        public MessageManagement(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }
        public async Task<Response<bool>> StoreUserMessage(UserMessage userMessage)
        {
            Response<bool> response = new Response<bool>();
            try
            {
                _sqlHelper.ExecuteStoredProcedure("usp_Store_Message", cmd =>
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
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
            }
            return response;    
        }
    }
}
