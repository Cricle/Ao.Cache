using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public abstract class RedisDataFinder<TIdentity, TEntity> : DataFinderBase<TIdentity, TEntity>
    {
        public abstract IDatabase GetDatabase();

        public override Task<bool> DeleteAsync(TIdentity identity)
        {
            return GetDatabase().KeyDeleteAsync(GetEntryKey(identity));
        }

        public override Task<bool> ExistsAsync(TIdentity identity)
        {
            return GetDatabase().KeyExistsAsync(GetEntryKey(identity));
        }

        public override Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            return GetDatabase().KeyExpireAsync(GetEntryKey(identity), time);
        }
    }
}
