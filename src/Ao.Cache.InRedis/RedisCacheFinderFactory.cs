using StackExchange.Redis;
using System;

namespace Ao.Cache.InRedis
{
    public class RedisDataFinderFactory : IBatchDataFinderFactory, IDataFinderFactory
    {
        public RedisDataFinderFactory(IDatabase database, IEntityConvertor entityConvertor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public IDatabase Database { get; }

        public IEntityConvertor EntityConvertor { get; }

        public IBatchDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IBatchDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultBitRedisBatchFinder<TIdentity, TEntity>(Database, accesstor, EntityConvertor);
        }

        public IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultBitRedisDataFinder<TIdentity, TEntity>(Database, accesstor, EntityConvertor);
        }
    }
}
