using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Ao.Cache.TextJson.Redis
{
    public abstract class RedisJsonDataFinder<TIdentity, TEntry> : JsonDataFinder<TIdentity, TEntry>
    {
        protected RedisJsonDataFinder()
        {
        }

        protected abstract IDatabase GetDatabase();

        protected override async Task<TEntry> CoreFindInCacheAsync(string key, TIdentity identity)
        {
            var val = await GetDatabase().StringGetAsync(key);
            if (val.HasValue)
            {
                return ToEntry(val);
            }
            return default;
        }

        protected override Task<bool> WriteCacheAsync(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
        {
            var buffer = ToBytes(entity);
            return GetDatabase().StringSetAsync(key, buffer, caheTime);
        }
        public override Task<bool> ExistsAsync(TIdentity identity)
        {
            return GetDatabase().KeyExistsAsync(GetEntryKey(identity));
        }
        public override Task<bool> DeleteAsync(TIdentity identity)
        {
            return GetDatabase().KeyDeleteAsync(GetEntryKey(identity));
        }
        public override Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            return GetDatabase().KeyExpireAsync(GetEntryKey(identity), time);
        }
    }

}
