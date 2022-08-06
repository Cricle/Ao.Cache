using StackExchange.Redis;
using System;

namespace Ao.Cache.InRedis
{
    public class RedisDataFinderFactory<TIdentity, TEntity> : IBatchDataFinderFactory<TIdentity, TEntity>, IDataFinderFactory<TIdentity, TEntity>
    {
        public RedisDataFinderFactory(IDatabase database, IEntityConvertor<TEntity> entityConvertor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public IDatabase Database { get; }

        public IEntityConvertor<TEntity> EntityConvertor { get; }

        public IBatchDataFinder<TIdentity, TEntity> Create(IBatchDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultBitRedisBatchFinder<TIdentity, TEntity>(Database, accesstor, EntityConvertor);
        }

        public IDataFinder<TIdentity, TEntity> Create(IDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultBitRedisDataFinder<TIdentity, TEntity>(Database, accesstor, EntityConvertor);
        }
    }
}
