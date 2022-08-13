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

        public IBatchDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IBatchDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultBitRedisBatchFinder<TIdentity, TEntity>(Connection.GetDatabase(), accesstor, EntityConvertor);
        }

        public IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultBitRedisDataFinder<TIdentity, TEntity>(Connection.GetDatabase(), accesstor, EntityConvertor);
        }
    }
}
