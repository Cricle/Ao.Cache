using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.InMemory
{
    public abstract class InMemoryCacheFinder<TIdentity, TEntry> : DataFinderBase<TIdentity, TEntry>
    {
        private static readonly Task<bool> trueTask = Task.FromResult(true);

        protected abstract IMemoryCache GetMemoryCache();

        public override Task<bool> DeleteAsync(TIdentity identity)
        {
            GetMemoryCache().Remove(GetEntryKey(identity));
            return trueTask;
        }

        public override Task<bool> ExistsAsync(TIdentity identity)
        {
            var r = GetMemoryCache().TryGetValue(GetEntryKey(identity),out _);
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
            if (GetMemoryCache().TryGetValue<TEntry>(key, out var data))
            {
                return Task.FromResult(data);
            }
            return Task.FromResult<TEntry>(default);
        }

        protected override Task<bool> SetInCahceAsync(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
        {
            var options = GetMemoryCacheEntryOptions(identity, caheTime);
            GetMemoryCache().Set(key, entity, options);
            return trueTask;
        }
    }
}
