using Microsoft.Extensions.Caching.Memory;

namespace CCIFunctionApp
{
    public class CacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public T GetOrCreate<T>(string key, Func<ICacheEntry, T> createItem)
        {
            return _memoryCache.GetOrCreate(key, createItem);
        }
    }
}
