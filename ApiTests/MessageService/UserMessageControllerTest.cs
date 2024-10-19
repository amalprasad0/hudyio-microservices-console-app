using MessageManagementService.Models;
using MessageManagementService.Controllers;
using MessageManagementService.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiTests.MessageService
{
    
    public  class UserMessageControllerTest
    {
        private readonly Mock<IMessageManagement> _managementService;
        private readonly Mock<IDBCacheService> _dbcacheService;
        private readonly UserMessageController _userMessageController;
        public UserMessageControllerTest() { 
            _dbcacheService = new Mock<IDBCacheService>();
            _managementService = new Mock<IMessageManagement>();
            _userMessageController= new UserMessageController( _managementService.Object,_dbcacheService.Object );
        }
        [Fact]
        public void Get_Return_Ok_StoreUserMessage()
        {
            Response<bool> response = new Response<bool>
            {
                Success = true,
                Data = true,
            };

            var userMessage = new UserMessage
            {
                messageContent = "hello test user",
                cachedMessageId="abc23",
                fileType=string.Empty,
                fileUrl=string.Empty,
                fromUserId=1,
                hasFile=false,
                toUserId=2,
            };
            _managementService.Setup(repo=>repo.StoreUserMessage(userMessage)).Returns(Task.FromResult(response));
            Assert.True(response.Success);
            Assert.True(response.Data);
            Assert.Null(response.ErrorMessage);
        }
        [Fact]
        public void Get_Return_false_StoreUserMessage()
        {
            Response<bool> response = new Response<bool>
            {
                Success = true,
                Data = true,
            };

            var userMessage = new UserMessage();
            
            _managementService.Setup(repo => repo.StoreUserMessage(userMessage)).Returns(Task.FromResult(response));
            Assert.True(response.Success);
            Assert.True(response.Data);
            Assert.NotEmpty(response.ErrorMessage);
        }
        [Fact]
        public void Get_Return_Ok_StorecachedMesage()
        {

            var cachedMessageIds = new List<string> { "MessageId1", "MessageId2" }; 
            var response = new Response<bool>
            {
                Success = true,
                Data = true,
                ErrorMessage = null
            };

            _managementService.Setup(respo => respo.StoreCachedMessage(cachedMessageIds))
                              .Returns(response);

           
            var result = _userMessageController.StoreCachedMessage(cachedMessageIds) as OkObjectResult;

          
            Assert.NotNull(result); 
            Assert.Equal(200, result.StatusCode); 
            var returnedResponse = result.Value as Response<bool>;
            Assert.NotNull(returnedResponse); 
            Assert.True(returnedResponse.Success);
            Assert.True(returnedResponse.Data);
            Assert.Null(returnedResponse.ErrorMessage); 
        }
        [Fact]
        public async Task Get_return_Ok_GetDBCacheUserIds()
        {
            
            var response = new Response<List<CachedUserIds>>();
            var cacheIds = new List<CachedUserIds>
    {
        new CachedUserIds
        {
            cacheIds = new List<string> { "ABC", "DFG" },
            userId = 1
        }
    };
            response.Data = cacheIds;

           
            _dbcacheService.Setup(repo => repo.GetDBCachedUserIds())
                           .ReturnsAsync(response);

           
            var result = await _userMessageController.GetDbCachedUserIds() as OkObjectResult;

           
            Assert.NotNull(result); 
            Assert.Equal(200, result.StatusCode);

            var returnedResponse = result.Value as Response<List<CachedUserIds>>;
            Assert.NotNull(returnedResponse); 
            Assert.Equal(cacheIds.Count, returnedResponse.Data.Count);
            Assert.Equal(cacheIds[0].userId, returnedResponse.Data[0].userId); 
            Assert.Equal("ABC", returnedResponse.Data[0].cacheIds[0]); 
            Assert.Equal("DFG", returnedResponse.Data[0].cacheIds[1]); 
        }
        [Fact]
        public async void Get_Return_Ok_GetDbCachedmessages()
        {
            var userId=1;
            Response<List<DBCachedMessages>> response = new Response<List<DBCachedMessages>>();
            var cachedMessage= new List<DBCachedMessages>();
            cachedMessage.Add(new DBCachedMessages
            {
                messageContent="hello",
                messageId="abc123",
                messageTime=DateTime.Now,
                sendByUser="1"
            });
            response.Data=cachedMessage;
            _dbcacheService.Setup(repo => repo.GetDBCachedMessages(userId)).Returns(Task.FromResult(response));
           var result= await  _userMessageController.GETDBCachedMessages(userId) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal(response.Data, cachedMessage);
        }

    }
}
