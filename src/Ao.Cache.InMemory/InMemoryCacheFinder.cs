using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InMemory
{
    public class InMemoryCacheFinder<TIdentity, TEntry> : DataFinderBase<TIdentity, TEntry>
    {
        private static readonly Task<bool> trueTask = Task.FromResult(true);
        private static readonly Task<bool> falseTask = Task.FromResult(false);

        public InMemoryCacheFinder(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        internal readonly IMemoryCache memoryCache;

        public IMemoryCache MemoryCache => memoryCache;

        public override Task<bool> DeleteAsync(TIdentity identity)
        {
            memoryCache.Remove(GetEntryKey(identity));
            return trueTask;
        }

        public override Task<bool> ExistsAsync(TIdentity identity)
        {
            var r = memoryCache.TryGetValue(GetEntryKey(identity), out _);
            return r? trueTask:falseTask;
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
           return Task.FromResult(CoreFindInCache(key, identity));
        }

        protected override Task<bool> SetInCacheAsync(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
        {
            SetInCache(key,identity, entity, caheTime);
            return trueTask;
        }
        public override Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            return Task.FromResult(Renewal(identity, time));
        }

        public override bool Delete(TIdentity identity)
        {
            memoryCache.Remove(GetEntryKey(identity));
            return true;
        }

        public override bool Exists(TIdentity identity)
        {
            return memoryCache.TryGetValue(GetEntryKey(identity), out _);
        }

        protected override bool SetInCache(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
        {
            var options = GetMemoryCacheEntryOptions(identity, caheTime);
            memoryCache.Set(key, entity, options);
            return true;
        }

        protected override TEntry CoreFindInCache(string key, TIdentity identity)
        {
            if (memoryCache.TryGetValue<TEntry>(key, out var data))
            {
                return data;
            }
            return default;
        }

        public override bool Renewal(TIdentity identity, TimeSpan? time)
        {
            var key = GetEntryKey(identity);
            var val = memoryCache.Get(key);
            if (val == null)
            {
                return false;
            }
            var options = GetMemoryCacheEntryOptions(identity, time);
            memoryCache.Set(key, val, options);
            return true;
        }
    }
}
