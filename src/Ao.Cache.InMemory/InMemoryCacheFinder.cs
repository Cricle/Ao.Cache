using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InMemory
{
    public abstract class InMemoryCacheFinder<TIdentity, TEntry> : DataFinderBase<TIdentity, TEntry>
    {
        private static readonly Task<bool> trueTask = Task.FromResult(true);

        protected InMemoryCacheFinder(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        private readonly IMemoryCache memoryCache;

        public IMemoryCache MemoryCache => memoryCache;

        public override Task<bool> DeleteAsync(TIdentity identity)
        {
            memoryCache.Remove(GetEntryKey(identity));
            return trueTask;
        }

        public override Task<bool> ExistsAsync(TIdentity identity)
        {
            var r = memoryCache.TryGetValue(GetEntryKey(identity), out _);
            return Task.FromResult(r);
        }

        protected virtual MemoryCacheEntryOptions GetMemoryCacheEntryOptions(TIdentity identity, TimeSpan? time)
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = time
            };
        }
        protected override Task<TEntry> CoreFindInCacheAsync(string key, TIdentity identity)
        {
            if (memoryCache.TryGetValue<TEntry>(key, out var data))
            {
                return Task.FromResult(data);
            }
            return Task.FromResult<TEntry>(default);
        }

        protected override Task<bool> SetInCacheAsync(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
        {
            var options = GetMemoryCacheEntryOptions(identity, caheTime);
            memoryCache.Set(key, entity, options);
            return trueTask;
        }
        public override Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            var key = GetEntryKey(identity);
            var val = memoryCache.Get(key);
            if (val == null)
            {
                return Task.FromResult(false);
            }
            var options = GetMemoryCacheEntryOptions(identity, time);
            memoryCache.Set(key, val, options);
            return Task.FromResult(true);
        }

    }
}
