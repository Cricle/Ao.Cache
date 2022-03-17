using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.MessagePack.Redis
{
    public abstract class RedisMessagePackDataFinder<TIdentity, TEntry> : MessagePackDataFinder<TIdentity, TEntry>
    {
        protected RedisMessagePackDataFinder(IDatabase database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public IDatabase Database { get; }

        protected override async Task<TEntry> CoreFindInCacheAsync(string key, TIdentity identity)
        {
            var val = await Database.StringGetAsync(key);
            if (val.HasValue)
            {
                return ToEntry(val);
            }
            return default;
        }

        protected override Task<bool> WriteCacheAsync(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
        {
            var buffer = ToBytes(entity);
            return Database.StringSetAsync(key, buffer, caheTime);
        }

        public virtual Task<bool> ExistsCacheAsync(TIdentity identity)
        {
            return Database.KeyExistsAsync(GetEntryKey(identity));
        }

        public virtual Task<bool> DeleteCahceAsync(TIdentity identity)
        {
            return Database.KeyDeleteAsync(GetEntryKey(identity));
        }
    }
}
