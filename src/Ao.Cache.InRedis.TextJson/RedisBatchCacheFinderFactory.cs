using StackExchange.Redis;
using System;

namespace Ao.Cache.InRedis.TextJson
{
    public class RedisTextJsonPackCacheFinderFactory<TIdentity, TEntry> : IBatchDataFinderFactory<TIdentity, TEntry>,IDataFinderFactory<TIdentity,TEntry>
    {
        public RedisTextJsonPackCacheFinderFactory(IDatabase database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public IDatabase Database { get; }

        public IBatchDataFinder<TIdentity, TEntry> Create(IBatchDataAccesstor<TIdentity, TEntry> accesstor)
        {
            return new DefaultBatchRedisJsonDataFinder<TIdentity, TEntry>(Database, accesstor);
        }

        public IDataFinder<TIdentity, TEntry> Create(IDataAccesstor<TIdentity, TEntry> accesstor)
        {
            return new DefaultRedisJsonDataFinder<TIdentity, TEntry>(Database, accesstor);
        }
    }
}
