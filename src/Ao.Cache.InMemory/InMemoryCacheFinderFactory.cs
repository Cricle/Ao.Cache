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

        public IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>()
        {
            return new InMemoryCacheFinder<TIdentity, TEntity>(MemoryCache);
        }

        public IWithDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultInMemoryCacheFinder<TIdentity, TEntity>(MemoryCache, accesstor);
        }

        public IWithBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>(IBatchDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultInMemoryBatchCacheFinder<TIdentity, TEntity>(MemoryCache, accesstor);
        }

        public IBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>()
        {
            return new InMemoryBatchCacheFinder<TIdentity, TEntity>(MemoryCache);
        }
    }
}
