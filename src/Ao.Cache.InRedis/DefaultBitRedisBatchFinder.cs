using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class DefaultSyncBitRedisBatchFinder<TIdentity, TEntity> : BitRedisBatchFinder<TIdentity, TEntity>, ISyncWithBatchDataFinder<TIdentity, TEntity>
    {
        public DefaultSyncBitRedisBatchFinder(IConnectionMultiplexer multiplexer,
            ISyncBatchDataAccesstor<TIdentity, TEntity> dataAccesstor,
            IEntityConvertor entityConvertor)
            : base(multiplexer, entityConvertor)
        {
            if (entityConvertor is null)
            {
                throw new ArgumentNullException(nameof(entityConvertor));
            }

            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public ISyncBatchDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        public IDictionary<TIdentity, TEntity> FindInDb(IReadOnlyList<TIdentity> identity, bool cache)
        {
            var entity = DataAccesstor.Find(identity);
            if (cache && entity != null)
            {
                SetInCache(entity);
            }
            return entity;
        }
    }
    public class DefaultBitRedisBatchFinder<TIdentity, TEntity> : BitRedisBatchFinder<TIdentity, TEntity>, IWithBatchDataFinder<TIdentity, TEntity>
    {
        public DefaultBitRedisBatchFinder(IConnectionMultiplexer multiplexer,
            IBatchDataAccesstor<TIdentity, TEntity> dataAccesstor,
            IEntityConvertor entityConvertor)
            : base(multiplexer, entityConvertor)
        {
            if (entityConvertor is null)
            {
                throw new ArgumentNullException(nameof(entityConvertor));
            }

            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IBatchDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        public async Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IReadOnlyList<TIdentity> identity, bool cache)
        {
            var entity=await DataAccesstor.FindAsync(identity);
            if (cache&&entity!=null)
            {
                await SetInCacheAsync(entity);
            }
            return entity;
        }
    }
}
