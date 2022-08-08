using Microsoft.Extensions.Caching.Memory;
using System;

namespace Ao.Cache.InMemory
{
    public class InMemoryCacheFinderFactory : IDataFinderFactory, IBatchDataFinderFactory
    {
        public InMemoryCacheFinderFactory(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public IMemoryCache MemoryCache { get; }

        public IDataFinder<TIdentity, TEntry> Create<TIdentity, TEntry>(IDataAccesstor<TIdentity, TEntry> accesstor)
        {
            return new DefaultInMemoryCacheFinder<TIdentity, TEntry>(MemoryCache, accesstor);
        }

        public IBatchDataFinder<TIdentity, TEntry> Create<TIdentity, TEntry>(IBatchDataAccesstor<TIdentity, TEntry> accesstor)
        {
            return new DefaultInMemoryBatchCacheFinder<TIdentity, TEntry>(MemoryCache, accesstor);
        }
    }
}
