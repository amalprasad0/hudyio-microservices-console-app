using Microsoft.AspNetCore.Mvc;
using CacheService.Interfaces;
using System;
using CacheService.Models;
using CacheService.Services;

namespace CacheService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IRedisCache _cacheRepository;
        private readonly IQueueCache _queueCache;
        private readonly IGetCachedData _getCachedData;
        private readonly ISyncDataToSql _syncDataToSql;
        private readonly IGetDBCacheUsercs _getDBCacheUsers;

        public CacheController(IRedisCache cacheRepository, IQueueCache queueCache, IGetCachedData getCachedData,ISyncDataToSql syncDataToSql,IGetDBCacheUsercs getDBCacheUsercs)
        {
            _cacheRepository = cacheRepository;
            _queueCache = queueCache;
            _getCachedData = getCachedData;
            _syncDataToSql = syncDataToSql;
            _getDBCacheUsers= getDBCacheUsercs;
        }

        [HttpGet("{key}")]
        public IActionResult Get(string key)
        {
            var value = _cacheRepository.GetCacheData(key);
            if (value == null)
            {
                return NotFound();
            }
            return new ObjectResult(value);
        }


        [HttpPost("set")]
        public IActionResult Set([FromBody] CacheEntry entry)
        {
            var isSet = _cacheRepository.SetCacheData(entry.Key, entry.Value, entry.Expiration);
            if (!isSet)
            {
                return StatusCode(500, "Error setting cache data");
            }
            return Ok(isSet);
        }

        [HttpDelete("{key}")]
        public IActionResult Remove(string key)
        {
            var isRemoved = _cacheRepository.RemoveData(key);
            if (!(bool)isRemoved)
            {
                return NotFound();
            }
            return Ok(isRemoved);
        }
        [HttpPost("SaveMessages")]
        public IActionResult AddQueueMessage(SaveRecentMessages saveRecentMessages)
        {
            if (string.IsNullOrEmpty(saveRecentMessages.ToUserId) || saveRecentMessages.MessageData == null )
            {
                return BadRequest("Key or messages are empty");
            }

           Response<bool> response = _queueCache.EnqueueMessage(saveRecentMessages);
            return Ok(response);
        }
        [HttpGet("GetMessages")]
        public IActionResult DequeueMessage(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest("Key is empty");
            }

            var dequeuedMessage = _queueCache.GetAllMessages<UserMessage>(key);
            if (dequeuedMessage != null)
            {
                return Ok(dequeuedMessage);
            }
            else
            {
                return NotFound("No messages in the queue or key does not exist");
            }
        }
        [HttpPost("Removecachedmessage")]
        public IActionResult RemoveCachedMessage(CachedMessageRemoval messageRemoval)
        {
            if(string.IsNullOrEmpty(messageRemoval.userId) || string.IsNullOrEmpty(messageRemoval.messageId))
            {
                return BadRequest("Request data is null");
            }
           Response<bool> response= _queueCache.DequeueMessageById(messageRemoval.userId, messageRemoval.messageId);
            return Ok(response);
        }
        [HttpGet("SyncCachedData")]
        public IActionResult SqlCacheMigration()
        {
            Response<bool> response = new Response<bool>();
          var isSuccess=_syncDataToSql.SyncDataToSql();
            response.success=true;
            response.data = isSuccess;
          return Ok(response);
        }
        [HttpGet("RemoveCachedMessages")]
        public async Task<IActionResult> TestAPI()
        {
            var result = await _getDBCacheUsers.RemoveDbCachedMSg();
            return Ok(result);
        }

        }
    }

