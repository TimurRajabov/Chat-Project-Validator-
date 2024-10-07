using Microsoft.Extensions.Caching.Memory;

namespace Chat.Api.Managers;

public class MemoryCacheManager
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheManager(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }   
    
    
    public  void GetOrUpdateDtos(string key, object dtos)
    {
        _memoryCache.Set(key, dtos);
    }

    public object? GetDtos(string key)
    {
        if (_memoryCache.TryGetValue(key, out object value))
        {
            
            return value;
        }

        return null;
    }

}