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
            return new DefaultBitRedisBatchFinder<TIdentity, TEntity>(Connection.GetDatabase(), accesstor, EntityConvertor);
        }

        public IWithDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultBitRedisDataFinder<TIdentity, TEntity>(Connection.GetDatabase(), accesstor, EntityConvertor);
        }

        public IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>()
        {
            return new BitRedisDataFinder<TIdentity, TEntity>(Connection.GetDatabase(), EntityConvertor);
        }

        public IBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>()
        {
            return new BitRedisBatchFinder<TIdentity, TEntity>(Connection.GetDatabase(), EntityConvertor);
        }

    }
}
