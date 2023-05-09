using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class BitRedisDataFinder<TIdentity, TEntity> : DataFinderBase<TIdentity, TEntity>
    {
        private static readonly Type EntityType = typeof(TEntity);
        public BitRedisDataFinder(IConnectionMultiplexer multiplexer,
            IEntityConvertor entityConvertor)
        {
            Multiplexer = multiplexer ?? throw new ArgumentNullException(nameof(multiplexer));
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public IEntityConvertor EntityConvertor { get; }

        public IConnectionMultiplexer Multiplexer { get; }

        public override bool Delete(TIdentity identity)
        {
            return Multiplexer.GetDatabase().KeyDelete(GetEntryKey(identity));
        }

        public override Task<bool> DeleteAsync(TIdentity identity)
        {
            return Multiplexer.GetDatabase().KeyDeleteAsync(GetEntryKey(identity));
        }

        public override bool Exists(TIdentity identity)
        {
            return Multiplexer.GetDatabase().KeyExists(GetEntryKey(identity));
        }

        public override Task<bool> ExistsAsync(TIdentity identity)
        {
            return Multiplexer.GetDatabase().KeyExistsAsync(GetEntryKey(identity));
        }

        public override bool Renewal(TIdentity identity, TimeSpan? time)
        {
            return Multiplexer.GetDatabase().KeyExpire(GetEntryKey(identity), time);
        }

        public override Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            return Multiplexer.GetDatabase().KeyExpireAsync(GetEntryKey(identity), time);
        }

        protected override TEntity CoreFindInCache(string key, TIdentity identity)
        {
            var data = Multiplexer.GetDatabase().StringGet(key);
            if (data.HasValue)
            {
                var memory = (ReadOnlyMemory<byte>)data;
                return (TEntity)EntityConvertor.ToEntry(memory, EntityType);
            }
            return default;
        }

        protected override async Task<TEntity> CoreFindInCacheAsync(string key, TIdentity identity)
        {
            var data = await Multiplexer.GetDatabase().StringGetAsync(key);
            if (data.HasValue)
            {
                var memory = (ReadOnlyMemory<byte>)data;
                return (TEntity)EntityConvertor.ToEntry(memory, EntityType);
            }
            return default;
        }

        protected override bool SetInCache(string key, TIdentity identity, TEntity entity, TimeSpan? caheTime)
        {
            var bs = EntityConvertor.ToBytes(entity, EntityType);

            return Multiplexer.GetDatabase().StringSet(key, bs, caheTime);
        }

        protected override Task<bool> SetInCacheAsync(string key, TIdentity identity, TEntity entity, TimeSpan? caheTime)
        {
            var bs = EntityConvertor.ToBytes(entity, EntityType);

            return Multiplexer.GetDatabase().StringSetAsync(key, bs, caheTime);
        }
    }
}
