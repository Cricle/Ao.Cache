using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class BitRedisBatchFinder<TIdentity, TEntity> : BatchDataFinderBase<TIdentity, TEntity>
    {
        public BitRedisBatchFinder(IConnectionMultiplexer multiplexer,
            IEntityConvertor entityConvertor)
        {
            Multiplexer = multiplexer ?? throw new ArgumentNullException(nameof(multiplexer));
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public IEntityConvertor EntityConvertor { get; }

        public IConnectionMultiplexer Multiplexer { get; }

        protected virtual RedisKey[] AsKeys(IReadOnlyList<TIdentity> identities)
        {
            var keys = new RedisKey[identities.Count];
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = GetEntryKey(identities[i]);
            }
            return keys;
        }
        protected virtual IDictionary<RedisKey, TIdentity> AsKeyMap(IReadOnlyList<TIdentity> identities)
        {
            var keys = new Dictionary<RedisKey, TIdentity>(identities.Count);
            for (int i = 0; i < identities.Count; i++)
            {
                keys[GetEntryKey(identities[i])] = identities[i];
            }
            return keys;
        }
        protected IDictionary<TIdentity, TResult> DoInRedis<TResult>(IReadOnlyList<TIdentity> identities,
            Func<TIdentity, TResult> fetch)
        {
            var map = new Dictionary<TIdentity, TResult>(identities.Count);
            for (int i = 0; i < identities.Count; i++)
            {
                map[identities[i]] = fetch(identities[i]);
            }
            return map;
        }
        protected async Task<IDictionary<TIdentity, TResult>> DoInRedisAsync<TResult>(IReadOnlyList<TIdentity> identities,
            Func<IBatch, TIdentity, Task<TResult>> fetch)
        {
            var batch = Multiplexer.GetDatabase().CreateBatch();
            var tasks = new Task<TResult>[identities.Count];
            for (int i = 0; i < identities.Count; i++)
            {
                tasks[i] = fetch(batch, identities[i]);
            }
            batch.Execute();
            await Task.WhenAll(tasks);
            var map = new Dictionary<TIdentity, TResult>(identities.Count);
            for (int i = 0; i < identities.Count; i++)
            {
                var task = tasks[i];
                var identity = identities[i];
                map[identity] = task.Result;
            }
            return map;
        }
        public override Task<long> DeleteAsync(IReadOnlyList<TIdentity> identities)
        {
            var keys = new RedisKey[identities.Count];
            for (int i = 0; i < identities.Count; i++)
            {
                keys[i] = GetEntryKey(identities[i]);
            }
            return Multiplexer.GetDatabase().KeyDeleteAsync(keys);
        }

        public override Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identities)
        {
            return DoInRedisAsync(identities, (batch, identity) => batch.KeyExistsAsync(GetEntryKey(identity)));
        }

        public override async Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input)
        {
            var res = await DoInRedisAsync(input.Keys.ToList(),
                (batch, identity) => batch.KeyExpireAsync(GetEntryKey(identity), input[identity]));

            return res.Count;
        }

        protected override async Task<IDictionary<TIdentity, TEntity>> CoreFindInCacheAsync(IReadOnlyList<TIdentity> identity)
        {
            var keyMap = AsKeyMap(identity);
            var keys = keyMap.Keys.ToArray();
            var datas = await Multiplexer.GetDatabase().StringGetAsync(keys);
            var map = new Dictionary<TIdentity, TEntity>(keys.Length);
            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if (data.HasValue)
                {
                    map[keyMap[keys[i]]] = (TEntity)EntityConvertor.ToEntry(data, typeof(TEntity));
                }
            }
            return map;
        }

        public override async Task<long> SetInCacheAsync(IDictionary<TIdentity, TEntity> pairs)
        {
            var res = await DoInRedisAsync(pairs.Keys.ToList(), (batch, identity) =>
                    batch.StringSetAsync(GetEntryKey(identity), EntityConvertor.ToBytes(pairs[identity], typeof(TEntity)), GetCacheTime(identity)));
            return res.Count;
        }

        public override long Delete(IReadOnlyList<TIdentity> identities)
        {
            var keys = new RedisKey[identities.Count];
            for (int i = 0; i < identities.Count; i++)
            {
                keys[i] = GetEntryKey(identities[i]);
            }
            return Multiplexer.GetDatabase().KeyDelete(keys);
        }

        public override IDictionary<TIdentity, bool> Exists(IReadOnlyList<TIdentity> identities)
        {
            return DoInRedis(identities, (identity) => Multiplexer.GetDatabase().KeyExists(GetEntryKey(identity)));
        }

        public override long SetInCache(IDictionary<TIdentity, TEntity> pairs)
        {
            var res = DoInRedis(pairs.Keys.ToList(), (identity) =>
                    Multiplexer.GetDatabase().StringSet(GetEntryKey(identity), EntityConvertor.ToBytes(pairs[identity], typeof(TEntity)), GetCacheTime(identity)));
            return res.Count;
        }

        protected override IDictionary<TIdentity, TEntity> CoreFindInCache(IReadOnlyList<TIdentity> identity)
        {
            var keyMap = AsKeyMap(identity);
            var keys = keyMap.Keys.ToArray();
            var datas = Multiplexer.GetDatabase().StringGet(keys);
            var map = new Dictionary<TIdentity, TEntity>(keys.Length);
            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if (data.HasValue)
                {
                    map[keyMap[keys[i]]] = (TEntity)EntityConvertor.ToEntry(data, typeof(TEntity));
                }
            }
            return map;
        }

        public override long Renewal(IDictionary<TIdentity, TimeSpan?> input)
        {
            var res = DoInRedis(input.Keys.ToList(),
                (identity) => Multiplexer.GetDatabase().KeyExpire(GetEntryKey(identity), input[identity]));

            return res.Count;
        }
    }
}
