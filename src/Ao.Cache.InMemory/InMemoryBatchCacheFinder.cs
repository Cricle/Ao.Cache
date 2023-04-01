using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InMemory
{
    public class DefaultInMemoryBatchCacheFinder<TIdentity, TEntry> : InMemoryBatchCacheFinder<TIdentity, TEntry>,IWithBatchDataFinder<TIdentity, TEntry>
    {
        public DefaultInMemoryBatchCacheFinder(IMemoryCache memoryCache, IBatchDataAccesstor<TIdentity, TEntry> dataAccesstor)
            :base(memoryCache)
        {
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IBatchDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        public async Task<IDictionary<TIdentity, TEntry>> FindInDbAsync(IReadOnlyList<TIdentity> identity, bool cache)
        {
            var entitys = await DataAccesstor.FindAsync(identity);
            if (entitys.Count!=0&&cache)
            {
                await SetInCacheAsync(entitys);
            }
            return entitys;
        }
    }
    public class InMemoryBatchCacheFinder<TIdentity, TEntry> : BatchDataFinderBase<TIdentity, TEntry>
    {
        public InMemoryBatchCacheFinder(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }
        private readonly IMemoryCache memoryCache;

        public IMemoryCache MemoryCache => memoryCache;

        public override Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity)
        {
            var res = 0L;
            foreach (var item in identity)
            {
                if (memoryCache.TryGetValue(item, out _))
                {
                    res++;
                    memoryCache.Remove(item);
                }
            }
            return Task.FromResult(res);
        }

        public override Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity)
        {
            var res = new Dictionary<TIdentity, bool>(identity.Count);
            foreach (var item in identity)
            {
                var key = GetEntryKey(item);
                res[item] = memoryCache.TryGetValue(key, out _);
            }
            return Task.FromResult<IDictionary<TIdentity, bool>>(res);
        }

        protected override Task<IDictionary<TIdentity, TEntry>> CoreFindInCacheAsync(IReadOnlyList<TIdentity> identity)
        {
            var res = new Dictionary<TIdentity, TEntry>(identity.Count);
            foreach (var item in identity)
            {
                var key = GetEntryKey(item);
                if (memoryCache.TryGetValue<TEntry>(key, out var r))
                {
                    res[item] = r;
                }
            }
            return Task.FromResult<IDictionary<TIdentity, TEntry>>(res);
        }

        public override Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input)
        {
            var res = 0L;
            foreach (var item in input)
            {
                var key = GetEntryKey(item.Key);
                var val = memoryCache.Get(key);
                if (val != null)
                {
                    var options = GetMemoryCacheEntryOptions(item.Key, item.Value);
                    memoryCache.Set(key, val, options);
                    res++;
                }
            }
            return Task.FromResult(res);
        }

        public override Task<long> SetInCacheAsync(IDictionary<TIdentity, TEntry> pairs)
        {
            var res = 0L;
            foreach (var item in pairs)
            {
                var time = GetCacheTime(item.Key);
                var options = GetMemoryCacheEntryOptions(item.Key, time);
                var key = GetEntryKey(item.Key);
                memoryCache.Set(key, item.Value, options);
                res++;
            }
            return Task.FromResult(res);
        }

        protected virtual MemoryCacheEntryOptions GetMemoryCacheEntryOptions(TIdentity identity, TimeSpan? time)
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = time
            };
        }
    }
}
