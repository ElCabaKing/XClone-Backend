using Microsoft.Extensions.Caching.Memory;
using X.Application.Interfaces;

namespace X.Infrastructure.Services;

public class CacheService(IMemoryCache memoryCache) : ICacheService
{
    public T Create<T>(string key, TimeSpan expiration, T value)
    {
        try
        {
            var create = memoryCache.GetOrCreate(key, (factory) =>
            {
                factory.SlidingExpiration = expiration;
                return value;
            });

            return create is null ? throw new Exception("Failed to create cache entry.") : create;
        }
        catch
        {
            throw;
        }
    }

    public bool Delete(string key)
    {
        try
        {
            memoryCache.Remove(key);
            return true;
        }
        catch
        {
            throw;
        }
    }

    public T? Get<T>(string key)
    {
        try
        {
            return memoryCache.Get<T>(key);
        }
        catch
        {
            throw;
        }
    }
}
