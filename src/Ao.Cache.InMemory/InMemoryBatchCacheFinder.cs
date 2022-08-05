using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InMemory
{
    public class DefaultInMemoryBatchCacheFinder<TIdentity, TEntry> : InMemoryBatchCacheFinder<TIdentity, TEntry>
    {
        public DefaultInMemoryBatchCacheFinder(IMemoryCache memoryCache, IBatchDataAccesstor<TIdentity, TEntry> dataAccesstor)
        {
            MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IMemoryCache MemoryCache { get; }

        public IBatchDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        protected override IMemoryCache GetMemoryCache()
        {
            return MemoryCache;
        }

        protected override Task<IDictionary<TIdentity, TEntry>> OnFindInDbAsync(IReadOnlyList<TIdentity> identities)
        {
            return DataAccesstor.FindAsync(identities);
        }
    }
    public abstract class InMemoryBatchCacheFinder<TIdentity, TEntry> : BatchDataFinderBase<TIdentity, TEntry>
    {
        protected abstract IMemoryCache GetMemoryCache();

        public override Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity)
        {
            var res = 0L;
            var mem = GetMemoryCache();
            foreach (var item in identity)
            {
                if (mem.TryGetValue(item, out _))
                {
                    res++;
                    mem.Remove(item);
                }
            }
            return Task.FromResult(res);
        }

        public override Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity)
        {
            var res = new Dictionary<TIdentity, bool>(identity.Count);
            var mem = GetMemoryCache();
            foreach (var item in identity)
            {
                var key = GetEntryKey(item);
                res[item] = mem.TryGetValue(key, out _);
            }
            return Task.FromResult<IDictionary<TIdentity, bool>>(res);
        }

        public override Task<IDictionary<TIdentity, TEntry>> FindInCahceAsync(IReadOnlyList<TIdentity> identity)
        {
            var res = new Dictionary<TIdentity, TEntry>(identity.Count);
            var mem = GetMemoryCache();
            foreach (var item in identity)
            {
                var key = GetEntryKey(item);
                if (mem.TryGetValue<TEntry>(key, out var r))
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
                var mem = GetMemoryCache();
                var val = mem.Get(key);
                if (val != null)
                {
                    var options = GetMemoryCacheEntryOptions(item.Key, item.Value);
                    mem.Set(key, val, options);
                    res++;
                }
            }
            return Task.FromResult(res);
        }

        public override Task<long> SetInCahceAsync(IDictionary<TIdentity, TEntry> pairs)
        {
            var res = 0L;
            var mem = GetMemoryCache();
            foreach (var item in pairs)
            {
                var time = GetCacheTime(item.Key, item.Value);
                var options = GetMemoryCacheEntryOptions(item.Key, time);
                var key = GetEntryKey(item.Key);
                mem.Set(key, item.Value, options);
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
