using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class DefaultSyncBitRedisDataFinder<TIdentity, TEntity> : BitRedisDataFinder<TIdentity, TEntity>, ISyncWithDataFinder<TIdentity, TEntity>
    {
        public DefaultSyncBitRedisDataFinder(IConnectionMultiplexer multiplexer,
            ISyncDataAccesstor<TIdentity, TEntity> dataAccesstor,
            IEntityConvertor entityConvertor)
            : base(multiplexer, entityConvertor)
        {
            if (entityConvertor is null)
            {
                throw new ArgumentNullException(nameof(entityConvertor));
            }

            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public ISyncDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        public TEntity FindInDb(TIdentity identity, bool cache)
        {
            var entity = DataAccesstor.Find(identity);
            if (cache && entity != null)
            {
                SetInCache(identity, entity);
            }
            return entity;
        }

    }
    public class DefaultBitRedisDataFinder<TIdentity, TEntity> : BitRedisDataFinder<TIdentity, TEntity>, IWithDataFinder<TIdentity, TEntity>
    {
        public DefaultBitRedisDataFinder(IConnectionMultiplexer multiplexer,
            IDataAccesstor<TIdentity, TEntity> dataAccesstor,
            IEntityConvertor entityConvertor)
            : base(multiplexer, entityConvertor)
        {
            if (entityConvertor is null)
            {
                throw new ArgumentNullException(nameof(entityConvertor));
            }

            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        public async Task<TEntity> FindInDbAsync(TIdentity identity, bool cache)
        {
            var entity = await DataAccesstor.FindAsync(identity);
            if (cache&&entity!=null)
            {
                await SetInCacheAsync(identity, entity);
            }
            return entity;
        }

    }
}
