using System.Threading.Tasks;

namespace CacheService.Interfaces
{
    public interface IGetCachedData
    {
        Task<List<int>> GetCachedUserId();
        Task<List<string>> GetCachedMessageId();
    }
}
