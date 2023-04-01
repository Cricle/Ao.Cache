using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class DefaultBitRedisDataFinder<TIdentity, TEntity> : BitRedisDataFinder<TIdentity, TEntity>, IWithDataFinder<TIdentity, TEntity>
    {
        public DefaultBitRedisDataFinder(IDatabase database,
            IDataAccesstor<TIdentity, TEntity> dataAccesstor,
            IEntityConvertor entityConvertor)
            : base(database, entityConvertor)
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
