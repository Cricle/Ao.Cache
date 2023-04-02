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
            return InMemoryBatchCacheFinderInstances<TIdentity, TEntity>.Get(MemoryCache);
        }
        public IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>()
        {
            return InMemoryCacheFinderInstances<TIdentity, TEntity>.Get(MemoryCache);
        }
        static class InMemoryBatchCacheFinderInstances<TIdentity, TEntity>
        {
            private static InMemoryBatchCacheFinder<TIdentity, TEntity> Instance;

            public static readonly object Locker = new object();

            public static InMemoryBatchCacheFinder<TIdentity, TEntity> Get(IMemoryCache memoryCache)
            {
                if (Instance == null)
                {
                    lock (Locker)
                    {
                        if (Instance == null)
                        {
                            Instance = new InMemoryBatchCacheFinder<TIdentity, TEntity>(memoryCache);
                        }
                    }
                }
                return Instance;
            }
        }
        static class InMemoryCacheFinderInstances<TIdentity, TEntity>
        {
            private static InMemoryCacheFinder<TIdentity, TEntity> Instance;

            public static readonly object Locker = new object();

            public static InMemoryCacheFinder<TIdentity, TEntity> Get(IMemoryCache memoryCache)
            {
                if (Instance == null)
                {
                    lock (Locker)
                    {
                        if (Instance == null)
                        {
                            Instance = new InMemoryCacheFinder<TIdentity, TEntity>(memoryCache);
                        }
                    }
                }
                return Instance;
            }
        }
    }
}
