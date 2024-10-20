﻿using Microsoft.AspNetCore.Mvc;
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
               Response<int> resp =_userService.StoreUserAndSendOTP(userDto);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("CheckOtpandActivateUser")]
        public IActionResult CheckOTPandActivateUser([FromBody] LoginParams loginParams)
        {
            try
            {
                if (loginParams.otp == null || loginParams.userId == null)
                {
                    return BadRequest("Params is Null");

                }
                Response<bool> response = _userService.CheckOtpandRegisterUser(loginParams);
                return Ok(response);
            }
            catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("saveconnectionid")]
        public IActionResult SaveConnectionId(SaveConnectionId saveConnectionId)
        {
            try { 
                if(saveConnectionId.ConnectionId==null || saveConnectionId.userId==null)
                {
                    return BadRequest("Request Params is null");
                }
                Response<bool> response = _userService.SaveConnectionId(saveConnectionId);
                return Ok(response);
            } catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("removeconnectionid")]
        public IActionResult RemoveUserConnectionId(string userId)
        {
            try
            {
                if (userId == "")
                {
                    return BadRequest("userId is Empty");
                   
                }
                Response<bool> response =_userService.RemoveConnectionId(userId);
                return Ok(response);
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
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
