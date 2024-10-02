using MessageManagementService.Interface;
using MessageManagementService.Models;
using MessageManagementService.Services.DBCacheService;
using Microsoft.AspNetCore.Mvc;

namespace MessageManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMessageController:ControllerBase
    {
        private readonly IMessageManagement _messageManagement;
        private readonly IDBCacheService _dbcacheService;
        public UserMessageController(IMessageManagement messageManagement, IDBCacheService cacheService)
        {
           _messageManagement = messageManagement;
            _dbcacheService = cacheService;
        }
        [HttpPost]
        [Route("StoreMessage")]
        public IActionResult StoreUserMessage(UserMessage userMessage) 
        {
            if (userMessage == null){
                return BadRequest("Request Data is null");
            }
            var response = _messageManagement.StoreUserMessage(userMessage);
            return Ok(response);
        }
        [HttpPost]
        [Route("StoreCachedMessage")]
        public IActionResult StoreCachedMessage(List<string> cachedMessageIds)
        {
            if (cachedMessageIds == null) {
                return BadRequest("Request data is null");
            }
            var response = _messageManagement.StoreCachedMessage(cachedMessageIds);
            return Ok(response);
        }
        [HttpGet]
        [Route("GetDBCachedUserIds")]
        public async Task<IActionResult> GetDbCachedUserIds()
        {
            Response<List<CachedUserIds>> response = await _dbcacheService.GetDBCachedUserIds();
            return Ok(response);
        }
        [HttpGet]
        [Route("GetCachedMessages")]
        public async Task<IActionResult> GETDBCachedMessages(int userId)
        {
            Response<List<DBCachedMessages>> response=await _dbcacheService.GetDBCachedMessages(userId);
            return Ok(response);
        }
    }
}
