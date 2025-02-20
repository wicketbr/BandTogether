using System.Runtime.Caching;

namespace BandTogether;
/// <summary>
/// extends the CacheStore to create a Clear method to remove all cached items
/// </summary>
public static class CacheExtensions
{
    /// <summary>
    /// removes all cached items from the cache
    /// </summary>
    /// <param name="cache"></param>
    public static void Clear(this ObjectCache cache) => cache.Select(kvp => kvp.Key).ToList().ForEach(k => cache.Remove(k));
}

public static class CacheStore
{
    /// <summary>
    /// Removes a cached item from the cache.
    /// </summary>
    /// <param name="cacheKey">Name/Key for the cache</param>
    public static void Clear(string cacheKey)
    {
        MemoryCache.Default.Remove(cacheKey);
    }

    /// <summary>
    /// Removes all cached items from the cache.
    /// </summary>
    public static void ClearAll()
    {
        MemoryCache.Default.Clear();
    }

    /// <summary>
    /// Determines if a key exists in the current cache store.
    /// </summary>
    /// <param name="cacheKey">Name/Key for the cache</param>
    /// <returns>True if the item exists in the cache store.</returns>
    public static bool ContainsKey(string cacheKey)
    {
        var memCache = MemoryCache.Default;
        bool output = memCache.Contains(cacheKey);
        return output;
    }

    /// <summary>
    /// Retrieve an item from the cache
    /// </summary>
    /// <param name="cacheKey">Name/Key for the cache</param>
    /// <returns>Stored object from cache</returns>
    public static T? GetCachedItem<T>(string cacheKey)
    {
        dynamic? output = null;
        var memCache = MemoryCache.Default;

        if (memCache.Contains(cacheKey)) {
            output = (T)memCache.GetCacheItem(cacheKey).Value;
        }

        return output;
    }

    /// <summary>
    /// Store an item in the cache
    /// </summary>
    /// <param name="cacheKey">Name/Key for the cache</param>
    /// <param name="item">Object to store in the cache. If null, then the item is removed from the cache.</param>
    /// <param name="absoluteExpiration">The absolute expiration of the cache item. Default is beginning of next day.</param>
    public static void SetCacheItem(string cacheKey, object? item, DateTimeOffset? absoluteExpiration = null)
    {
        var memCache = MemoryCache.Default;
        // If the item is null, then clear this item

        if (item == null) {
            memCache.Remove(cacheKey);
        } else {
            var policy = new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration ?? DateTimeOffset.Now.AddHours(1.0) };
            var cItem = new CacheItem(cacheKey, item);
            memCache.Set(cItem, policy);
        }
    }
}
