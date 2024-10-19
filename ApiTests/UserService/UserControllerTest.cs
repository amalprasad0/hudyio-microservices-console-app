using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usermicroservices;
using UserMicroservices.Controllers;
using UserMicroservices.Interface;
using UserMicroservices.Models;
namespace ApiTests.UserService
{
    public class UserControllerTest
    {
        private readonly Mock<IUser> _userService;
        private readonly UserController _userController;
        public UserControllerTest()
        {
            _userService = new Mock<IUser>();
            _userController = new UserController(_userService.Object);

        }
        [Fact]
        public async Task Get_Return_Ok_CreateUser()
        {
            var response= new Response<int> { Data=1, Success=true};
            var user = new CreateUser
            {
                Name = "test User",
                Email = "test@gmail.com",
                Phone = "+978645365",
                IsDisabled = "0"
            };
            _userService.Setup(repo=>repo.StoreUserAndSendOTP(user)).Returns(response);
            var result =  _userController.CreateUser(user) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal(200,result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
            Assert.True(response.Success);
            Assert.IsType<int>(response.Data);
        }
         
    }
}
