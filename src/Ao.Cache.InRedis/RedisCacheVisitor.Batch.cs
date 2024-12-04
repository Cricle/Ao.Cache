using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public partial class RedisCacheVisitor
    {
        private async Task<long> BatchRunAsync<T>(Func<IBatch, IEnumerable<Task<T>>> creator)
        {
            var batch = Database.CreateBatch();
            var tasks = creator(batch);
            batch.Execute();
            var newTask = await Task.WhenAll(tasks);
            return tasks.Count(x => x.IsCompleted && x.Exception == null);
        }
        private RedisKey[] AsKeys(IReadOnlyList<string> keys)
        {
            var map = new RedisKey[keys.Count];
            for (int i = 0; i < keys.Count; i++)
            {
                map[i] = keys[i];
            }
            return map;
        }

        public long Exists(IReadOnlyList<string> keys)
        {
            var redisKeys = AsKeys(keys);
            return Database.KeyExists(redisKeys);
        }

        public async Task<long> ExistsAsync(IReadOnlyList<string> keys)
        {
            var redisKeys = AsKeys(keys);
            return await Database.KeyExistsAsync(redisKeys);
        }

        public IReadOnlyList<T> Get<T>(IReadOnlyList<string> keys)
        {
            var type = typeof(T);
            var redisKeys = AsKeys(keys);
            var res = Database.StringGet(redisKeys);
            return res.Select(x => (T)EntityConvertor.ToEntry(x, type)).ToList();
        }
        public async Task<IReadOnlyList<T>> GetAsync<T>(IReadOnlyList<string> keys)
        {
            var type=typeof(T);
            var redisKeys = AsKeys(keys);
            var res = await Database.StringGetAsync(redisKeys);
            return res.Select(x=>(T)EntityConvertor.ToEntry(x,type)).ToList();
        }

        public long Set<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var values = ToStringSet(datas);
            var res = Database.StringSet(values, (When)cacheSetIf);
            return res ? datas.Length : 0;
        }
        public async Task<long> SetAsync<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var values = ToStringSet(datas);
            var res = await Database.StringSetAsync(values, (When)cacheSetIf);
            return res ? datas.Length : 0;
        }

        private KeyValuePair<RedisKey, RedisValue>[] ToStringSet<T>(KeyValuePair<string,T>[] datas)
        {
            var type = typeof(T);
            var values = new KeyValuePair<RedisKey, RedisValue>[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                var item = datas[i];
                values[i] = new KeyValuePair<RedisKey, RedisValue>(item.Key, EntityConvertor.ToBytes(item.Value, type));
            }
            return values;
        }

        private KeyValuePair<RedisKey, RedisValue>[] ToStringSet(KeyValuePair<string, string>[] datas)
        {
            var values = new KeyValuePair<RedisKey, RedisValue>[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                var item = datas[i];
                values[i] = new KeyValuePair<RedisKey, RedisValue>(item.Key, item.Value);
            }
            return values;
        }

        public IReadOnlyList<string> GetString(IReadOnlyList<string> keys)
        {
            var redisKeys = AsKeys(keys);
            var results = Database.StringGet(redisKeys);
            return results.Select(x => x.ToString()).ToList();
        }

        public async Task<IReadOnlyList<string>> GetStringAsync(IReadOnlyList<string> keys)
        {
            var redisKeys = AsKeys(keys);
            var results=await Database.StringGetAsync(redisKeys);
            return results.Select(x => x.ToString()).ToList();
        }

        public long SetString(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var values = ToStringSet(datas);
            var res = Database.StringSet(values, (When)cacheSetIf);
            return res ? datas.Length : 0;
        }

        public async Task<long> SetStringAsync(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var values = ToStringSet(datas);
            var res = await Database.StringSetAsync(values, (When)cacheSetIf);
            return res ? datas.Length : 0;
        }

        public long Delete(IReadOnlyList<string> keys)
        {
            var redisKeys = AsKeys(keys);
            return Database.KeyDelete(redisKeys);
        }

        public async Task<long> DeleteAsync(IReadOnlyList<string> keys)
        {
            var redisKeys = AsKeys(keys);
            return await Database.KeyDeleteAsync(redisKeys);
        }

        public long Expire(IReadOnlyList<string> keys, TimeSpan? cacheTime)
        {
            return ExpireAsync(keys, cacheTime).GetAwaiter().GetResult();
        }

        public async Task<long> ExpireAsync(IReadOnlyList<string> keys, TimeSpan? cacheTime)
        {
            return await BatchRunAsync(x => keys.Select(y => x.KeyExpireAsync(y, cacheTime)));
        }
    }
}
