using MessageManagementService.Interface;
using MessageManagementService.Utilities;
using MessageManagementService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MessageManagementService.Services.DBCacheService
{
    public class DBCacheService : IDBCacheService
    {
        private readonly SqlHelper _sqlHelper;

        public DBCacheService(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        /*public async Task<Response<List<int>>> GetDBCachedUserIds()
        {
            Response<List<int>> response = new Response<List<int>>();
            try
            {
                List<int> userIds = await _sqlHelper.ExecuteStoredProcedureWithResultAsync<int>(
                    "usp_GetDbCachedUserIds",
                    null,
                    reader => reader.GetInt32(0) // Assuming the first column is the user ID (integer)
                );

                response.Data = userIds;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Success = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }*/
        public async Task<Response<List<CachedUserIds>>> GetDBCachedUserIds()
        {
            Response<List<CachedUserIds>> response = new Response<List<CachedUserIds>>()
            {
                Data = new List<CachedUserIds>()
            };

            try
            {
                await _sqlHelper.ExecuteStoredProcedureWithResultAsync("usp_GetDbCachedUserIds", null, reader =>
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                        {
                            CachedUserIds cachedUserIds = new CachedUserIds
                            {
                                userId = reader.GetInt32(0),  
                                cacheIds = reader.GetString(1).Split(',').ToList()  
                            };

                            response.Data.Add(cachedUserIds);  // Add to the response data list
                        }
                    }
                });

                if (response.Data.Count > 0)
                {
                    response.Success = true;
                }
                else
                {
                    response.ErrorMessage = "No data found.";
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = $"Exception: {ex.Message}";  
                response.Success = false;
            }

            return response;
        }
        public async Task<Response<List<DBCachedMessages>>> GetDBCachedMessages(int userId)
        {
            Response<List<DBCachedMessages>> response= new Response<List<DBCachedMessages>>(
                
                )
            {
                Data = new List<DBCachedMessages>(),  
                Success = true  
            }; 
            try
            {
                await _sqlHelper.ExecuteStoredProcedureWithResultAsync("usp_GetDBCachedMessages", cmd =>
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                }, reader =>
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                        {
                            DBCachedMessages dBCachedMessages = new DBCachedMessages
                            {
                                sendByUser = reader.GetInt32(0),
                                messageTime = reader.GetDateTime(1),
                                messageContent = reader.GetString(2),
                                messageId=reader.GetString(3),
                            };
                            response.Data.Add(dBCachedMessages);  
                        }
                    }
                });          }
            catch (Exception ex) { 
            response.Success= true;
            response.ErrorMessage= ex.Message;  
            }
            return response;
        }

    }
}
