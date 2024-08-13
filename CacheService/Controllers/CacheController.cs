using Microsoft.AspNetCore.Mvc;
using CacheService.Interfaces;
using System;
using CacheService.Models;

namespace CacheService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IRedisCache _cacheRepository;
        private readonly IQueueCache _queueCache;

        public CacheController(IRedisCache cacheRepository, IQueueCache queueCache)
        {
            _cacheRepository = cacheRepository;
            _queueCache = queueCache;
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
            if (string.IsNullOrEmpty(saveRecentMessages.userId) || saveRecentMessages.Messages == null || saveRecentMessages.Messages.Count < 1)
            {
                return BadRequest("Key or messages are empty");
            }

            bool success = _queueCache.EnqueueMessage(saveRecentMessages.userId,saveRecentMessages.Messages);
            if (success)
            {
                return Ok(true);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, false);
            }
        }
        [HttpGet("GetMessages")]
        public IActionResult DequeueMessage(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest("Key is empty");
            }

            var dequeuedMessage = _queueCache.DequeueMessage<List<string>>(key);
            if (dequeuedMessage != null)
            {
                return Ok(dequeuedMessage);
            }
            else
            {
                return NotFound("No messages in the queue or key does not exist");
            }
        }

    }


}
