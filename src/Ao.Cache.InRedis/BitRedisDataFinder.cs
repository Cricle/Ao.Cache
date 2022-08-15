using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public abstract class BitRedisDataFinder<TIdentity, TEntity> : DataFinderBase<TIdentity, TEntity>
    {
        protected BitRedisDataFinder(IDatabase database,
            IEntityConvertor entityConvertor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public IEntityConvertor EntityConvertor { get; }

        public IDatabase Database { get; }

        public override Task<bool> DeleteAsync(TIdentity identity)
        {
            return Database.KeyDeleteAsync(GetEntryKey(identity));
        }

        public override Task<bool> ExistsAsync(TIdentity identity)
        {
            return Database.KeyExistsAsync(GetEntryKey(identity));
        }

        public override Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            return Database.KeyExpireAsync(GetEntryKey(identity), time);
        }
        protected override async Task<TEntity> CoreFindInCacheAsync(string key, TIdentity identity)
        {
            var data = await Database.StringGetAsync(key);
            if (data.HasValue)
            {
                return (TEntity)EntityConvertor.ToEntry(data,typeof(TEntity));
            }
            return default;
        }

        protected override Task<bool> SetInCacheAsync(string key, TIdentity identity, TEntity entity, TimeSpan? caheTime)
        {
            var bs = EntityConvertor.ToBytes(entity, typeof(TEntity));

            return Database.StringSetAsync(key, bs, caheTime);
        }
    }
}
