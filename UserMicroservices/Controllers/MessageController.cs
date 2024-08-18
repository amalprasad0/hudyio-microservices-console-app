using Microsoft.AspNetCore.Mvc;
using UserMicroservices.Interface;
using UserMicroservices.Models;

namespace UserMicroservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController:ControllerBase
    {
        private readonly IMessage _messageService;
        public MessageController(IMessage messageService) { 
            _messageService = messageService;
        }
        [HttpPost]
        [Route("StoreMessage")]
        public IActionResult StoreMessage([FromBody] UserMessage userMessage) {
            if (userMessage == null) {
                return BadRequest("Request data is Null");
            }
            var response =_messageService.StoreUserMessage(userMessage);    
            return Ok(response);
        }
    }
}
