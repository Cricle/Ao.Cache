using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InMemory
{
    public class DefaultInMemoryCacheFinder<TIdentity, TEntry> : InMemoryCacheFinder<TIdentity, TEntry>, IWithDataAccesstorFinder<TIdentity, TEntry>
    {
        public DefaultInMemoryCacheFinder(IMemoryCache memoryCache, IDataAccesstor<TIdentity, TEntry> dataAccesstor)
        {
            MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IMemoryCache MemoryCache { get; }

        public IDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        protected override IMemoryCache GetMemoryCache()
        {
            return MemoryCache;
        }

        protected override Task<TEntry> OnFindInDbAsync(TIdentity identity)
        {
            return DataAccesstor.FindAsync(identity);
        }
        public override Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            var key = GetEntryKey(identity);
            var val = MemoryCache.Get(key);
            if (val == null)
            {
                return Task.FromResult(false);
            }
            var options = GetMemoryCacheEntryOptions(identity, time);
            MemoryCache.Set(GetEntryKey(identity), val, options);
            return Task.FromResult(true);
        }
    }
}
