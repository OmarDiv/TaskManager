using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;
using TaskManager.Application.Common.Interfaces.Services;

namespace TaskManager.Infrastructure.Services.Caching
{
    public class CacheService(IDistributedCache distributedCache, ILogger<CacheService> logger) : ICacheService
    {
        private static readonly ConcurrentDictionary<string, bool> _cachedKey = [];
        private readonly IDistributedCache _distributedCache = distributedCache;

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var Cache = await _distributedCache.GetStringAsync(key);
            if (Cache == null)
                return null;
            return JsonSerializer.Deserialize<T>(Cache);
        }

        public async Task<T?> GetAsync<T>(string key, Func<Task<T?>> factory, CancellationToken cancellationToken = default) where T : class
        {
            T? cachedValue = await GetAsync<T>(key, cancellationToken);
            if (cachedValue is not null)
            {
                logger.LogInformation("FromCache");
                return cachedValue;
            }
            cachedValue = await factory();

            if (cachedValue is not null)
            {
                await SetAsync(key, cachedValue, cancellationToken);
            }
            
            return cachedValue;
        }
        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
        {
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), cancellationToken);
            _cachedKey.TryAdd(key, false);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key);
            _cachedKey.TryRemove(key, out bool _);
        }
        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
        {
            IEnumerable<Task> tasks = _cachedKey.Keys
                .Where(k => k.StartsWith(prefixKey))
                .Select(k => RemoveAsync(k, cancellationToken));

            await Task.WhenAll(tasks);
        }
    }
}
