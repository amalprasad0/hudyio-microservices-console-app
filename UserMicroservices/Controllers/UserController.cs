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
                _userService.CreateUser(userDto);
                return Ok("User created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("loginUser")]
        public IActionResult LoginUser([FromBody] LoginUser userDto)
        {
            try
            {
                if (!string.IsNullOrEmpty(userDto.Username) || !string.IsNullOrEmpty(userDto.Password) || !string.IsNullOrEmpty(userDto.Email))
                {

                    bool isUserExists = _userService.loginUser(userDto);
                    return Ok(isUserExists);
                }
                else {
                    return BadRequest("User data is null.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("getUser")]
        public IActionResult GetUserDetails(string userEmail, string userName)
        {
            if (userEmail == null || userName == null)
            {
                return BadRequest("User data is null.");
            }
            List<UserData> userData =_userService.GetUserData(userEmail, userName);
            return Ok(userData);
        }
        [HttpPost]
        [Route("updateUserPassword")]
        public IActionResult UpdateUserPassword(string? userName, string? userEmail,string userPassword)
        {
            /*if (userEmail == null || userName == null || userPassword==null)
            {
                return BadRequest("User data is null.");
            }*/
            bool isUpdated=_userService.UpdatePassword(userEmail, userName, userPassword);
            return Ok(isUpdated);
        }
        [HttpGet]
        [Route("health")]
        public IActionResult checkHealth()
        {
            return Ok("Ok Health");
        }
    }
}
