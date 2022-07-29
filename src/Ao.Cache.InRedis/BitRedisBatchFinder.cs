using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public abstract class BitRedisBatchFinder<TIdentity, TEntity>: RedisBatchDataFinder<TIdentity, TEntity>
    {
        protected BitRedisBatchFinder(IEntityConvertor<TEntity> entityConvertor)
        {
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public IEntityConvertor<TEntity> EntityConvertor { get; }

        public override async Task<IDictionary<TIdentity, TEntity>> FindInCahceAsync(IReadOnlyList<TIdentity> identity)
        {
            var keyMap = AsKeyMap(identity);
            var keys = keyMap.Keys.ToArray();
            var db = GetDatabase();
            var datas = await db.StringGetAsync(keys);
            var map = new Dictionary<TIdentity, TEntity>(keys.Length);
            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if (data.HasValue)
                {
                    map[keyMap[keys[i]]] = EntityConvertor.ToEntry(data);
                }
            }
            return map;
        }

        public override async Task<long> SetInCahceAsync(IDictionary<TIdentity, TEntity> pairs)
        {
            var res=await DoInRedisAsync(pairs.Keys.ToList(), (batch, identity) =>
                    batch.StringSetAsync(GetEntryKey(identity), EntityConvertor.ToBytes(pairs[identity]), GetCacheTime(identity, pairs[identity])));
            return res.Count;
        }

    }
}
