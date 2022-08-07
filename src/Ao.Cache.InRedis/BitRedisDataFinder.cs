using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public abstract class BitRedisDataFinder<TIdentity, TEntity> : RedisDataFinder<TIdentity, TEntity>
    {
        protected BitRedisDataFinder(IEntityConvertor<TEntity> entityConvertor)
        {
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public IEntityConvertor<TEntity> EntityConvertor { get; }

        protected override async Task<TEntity> CoreFindInCacheAsync(string key, TIdentity identity)
        {
            var data = await GetDatabase().StringGetAsync(key);
            if (data.HasValue)
            {
                return EntityConvertor.ToEntry(data);
            }
            return default;
        }

        protected override Task<bool> SetInCacheAsync(string key, TIdentity identity, TEntity entity, TimeSpan? caheTime)
        {
            var bs = EntityConvertor.ToBytes(entity);

            return GetDatabase().StringSetAsync(key, bs, caheTime);
        }
    }
}
