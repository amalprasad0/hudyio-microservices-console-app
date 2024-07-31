using Microsoft.AspNetCore.Mvc;
using UserMicroservices.Interface;
using UserMicroservices.Models;

namespace UserMicroservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("CreateUser")]
        public IActionResult CreateUser([FromBody] CreateUser userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is null.");
            }

            try
            {
               Response<bool> resp =_userService.StoreUserAndSendOTP(userDto);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpGet]
        [Route("health")]
        public IActionResult checkHealth()
        {
            return Ok("Ok Health");
        }
    }
}
