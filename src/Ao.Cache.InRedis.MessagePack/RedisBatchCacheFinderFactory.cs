using StackExchange.Redis;
using System;

namespace Ao.Cache.InRedis.MessagePack
{
    public class RedisMessagePackCacheFinderFactory<TIdentity, TEntry> : IBatchDataFinderFactory<TIdentity, TEntry>, IDataFinderFactory<TIdentity, TEntry>
    {
        public RedisMessagePackCacheFinderFactory(IDatabase database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public IDatabase Database { get; }

        public IBatchDataFinder<TIdentity, TEntry> Create(IBatchDataAccesstor<TIdentity, TEntry> accesstor)
        {
            return new DefaultBatchRedisMessagePackDataFinder<TIdentity, TEntry>(Database, accesstor);
        }

        public IDataFinder<TIdentity, TEntry> Create(IDataAccesstor<TIdentity, TEntry> accesstor)
        {
            return new DefaultRedisMessagePackDataFinder<TIdentity, TEntry>(Database, accesstor);
        }
    }
}
