using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Ao.Cache.InRedis
{
    public abstract class RedisBatchDataFinder<TIdentity, TEntity> : BatchDataFinderBase<TIdentity, TEntity>
    {
        public abstract IDatabase GetDatabase();

        protected virtual RedisKey[] AsKeys(IReadOnlyList<TIdentity> identities)
        {
            var keys = new RedisKey[identities.Count];
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = GetEntryKey(identities[i]);
            }
            return keys;
        }
        protected virtual IDictionary<RedisKey,TIdentity> AsKeyMap(IReadOnlyList<TIdentity> identities)
        {
            var keys = new Dictionary<RedisKey, TIdentity>(identities.Count);
            for (int i = 0; i < identities.Count; i++)
            {
                keys[GetEntryKey(identities[i])] = identities[i];
            }
            return keys;
        }
        protected async Task<IDictionary<TIdentity, TResult>> DoInRedisAsync<TResult>(IReadOnlyList<TIdentity> identities,
            Func<IBatch,TIdentity,Task<TResult>> fetch)
        {
            var db = GetDatabase();
            var batch = db.CreateBatch();
            var tasks = new Task<TResult>[identities.Count];
            for (int i = 0; i < identities.Count; i++)
            {
                tasks[i] = fetch(batch,identities[i]);
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
        public override Task<IDictionary<TIdentity, bool>> DeleteAsync(IReadOnlyList<TIdentity> identities)
        {
            return DoInRedisAsync(identities, (batch, identity) => batch.KeyDeleteAsync(GetEntryKey(identity)));
        }

        public override Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identities)
        {
            return DoInRedisAsync(identities, (batch, identity) => batch.KeyExistsAsync(GetEntryKey(identity)));
        }

        public override Task<IDictionary<TIdentity, bool>> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input)
        {
            return DoInRedisAsync(input.Keys.ToList(),
                (batch, identity) => batch.KeyExpireAsync(GetEntryKey(identity), input[identity]));
        }

    }
}
