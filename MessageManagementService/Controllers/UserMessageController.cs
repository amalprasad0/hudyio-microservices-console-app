using MessageManagementService.Interface;
using MessageManagementService.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessageManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMessageController:ControllerBase
    {
        private readonly IMessageManagement _messageManagement;
        public UserMessageController(IMessageManagement messageManagement) 
        {
           _messageManagement = messageManagement;
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
    }
}
