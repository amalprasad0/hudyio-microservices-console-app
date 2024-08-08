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

        public CacheController(IRedisCache cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        [HttpGet("{key}")]
        public IActionResult Get(string key)
        {
            var value = _cacheRepository.GetCacheData<object>(key);
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
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
            return Ok();
        }
    }

   
}
