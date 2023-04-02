using StackExchange.Redis;
using System;

namespace Ao.Cache.InRedis
{
    public class RedisDataFinderFactory : IBatchDataFinderFactory, IDataFinderFactory
    {
        public RedisDataFinderFactory(IConnectionMultiplexer multiplexer, IEntityConvertor entityConvertor)
        {
            Connection = multiplexer ?? throw new ArgumentNullException(nameof(multiplexer));
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public IConnectionMultiplexer Connection { get; }

        public IEntityConvertor EntityConvertor { get; }

        public IWithBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>(IBatchDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultBitRedisBatchFinder<TIdentity, TEntity>(Connection, accesstor, EntityConvertor);
        }

        public IWithDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultBitRedisDataFinder<TIdentity, TEntity>(Connection, accesstor, EntityConvertor);
        }
        
        public IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>()
        {
            return BitRedisDataFinderInstances<TIdentity, TEntity>.Get(Connection, EntityConvertor);
        }

        public IBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>()
        {
            return BitRedisBatchFinderInstances<TIdentity, TEntity>.Get(Connection, EntityConvertor);
        }
        static class BitRedisBatchFinderInstances<TIdentity, TEntity>
        {
            private static BitRedisBatchFinder<TIdentity, TEntity> Instance;

            public static readonly object Locker = new object();

            public static BitRedisBatchFinder<TIdentity, TEntity> Get(IConnectionMultiplexer multiplexer, IEntityConvertor convertor)
            {
                if (Instance == null)
                {
                    lock (Locker)
                    {
                        if (Instance == null)
                        {
                            Instance = new BitRedisBatchFinder<TIdentity, TEntity>(multiplexer, convertor);
                        }
                    }
                }
                return Instance;
            }
        }
        static class BitRedisDataFinderInstances<TIdentity, TEntity>
        {
            private static BitRedisDataFinder<TIdentity, TEntity> Instance;

            public static readonly object Locker = new object();

            public static BitRedisDataFinder<TIdentity, TEntity> Get(IConnectionMultiplexer multiplexer,IEntityConvertor convertor)
            {
                if (Instance==null)
                {
                    lock (Locker)
                    {
                        if (Instance==null)
                        {
                            Instance = new BitRedisDataFinder<TIdentity, TEntity>(multiplexer,convertor);
                        }
                    }
                }
                return Instance;
            }
        }
    }
}
