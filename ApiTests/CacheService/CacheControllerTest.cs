using CacheService.Controllers;
using CacheService.Interfaces;
using CacheService.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ApiTests.CacheService
{
    public class CacheControllerTest
    {
        private readonly Mock<IRedisCache> _mockCacheRepository;
        private readonly Mock<IQueueCache> _mockQueueCache;
        private readonly Mock<IGetCachedData> _mockGetCachedData;
        private readonly Mock<ISyncDataToSql> _mockSyncDataToSql;
        private readonly Mock<IGetDBCacheUsercs> _mockGetDBCacheUsers;
        private readonly CacheController _controller;

        public CacheControllerTest()
        {
            _mockCacheRepository = new Mock<IRedisCache>();
            _mockQueueCache = new Mock<IQueueCache>();
            _mockGetCachedData = new Mock<IGetCachedData>();
            _mockSyncDataToSql = new Mock<ISyncDataToSql>();
            _mockGetDBCacheUsers = new Mock<IGetDBCacheUsercs>();

            _controller = new CacheController(
                _mockCacheRepository.Object,
                _mockQueueCache.Object,
                _mockGetCachedData.Object,
                _mockSyncDataToSql.Object,
                _mockGetDBCacheUsers.Object
            );
        }

        [Fact]
        public void Get_Returns_NotFound_WhenKeyDoesNotExist()
        {
            string key = "non-existing-key";
            _mockCacheRepository.Setup(repo => repo.GetCacheData(key)).Returns((string)null);

            var result = _controller.Get(key);

            
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Get_Returns_Value_WhenKeyExists()
        {
           
            string key = "existing-key";
            string expectedValue = "cached-value";
            _mockCacheRepository.Setup(repo => repo.GetCacheData(key)).Returns(expectedValue);

            var result = _controller.Get(key) as ObjectResult;

            Assert.IsType<ObjectResult>(result);
            Assert.Equal(expectedValue, result.Value);
        }
        [Fact]
        public void Set_Returns_Ok_WhenDataIsSetSuccessfully()
        {
            var cacheEntry= new CacheEntry { Key = "key1", Value = "value1", Expiration = DateTime.Now.AddMinutes(20) };
            _mockCacheRepository.Setup(repo => repo.SetCacheData(cacheEntry.Key, cacheEntry.Value, cacheEntry.Expiration))
                               .Returns(true);
            var result = _controller.Set(cacheEntry);
            Assert.IsType<OkObjectResult>(result);
        }

    }
}
