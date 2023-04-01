using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class DefaultBitRedisBatchFinder<TIdentity, TEntity> : BitRedisBatchFinder<TIdentity, TEntity>, IWithBatchDataFinder<TIdentity, TEntity>
    {
        public DefaultBitRedisBatchFinder(IDatabase database,
            IBatchDataAccesstor<TIdentity, TEntity> dataAccesstor,
            IEntityConvertor entityConvertor)
            : base(database, entityConvertor)
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
