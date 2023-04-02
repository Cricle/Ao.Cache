using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public partial class RedisCacheVisitor : ICacheVisitor
    {
        public RedisCacheVisitor(IDatabase database, IEntityConvertor entityConvertor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public IDatabase Database { get; }

        public IEntityConvertor EntityConvertor { get; }

        public bool Delete(string key)
        {
            return Database.KeyDelete(key);
        }

        public Task<bool> DeleteAsync(string key)
        {
            return Database.KeyDeleteAsync(key);
        }

        public bool Exists(string key)
        {
            return Database.KeyExists(key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Database.KeyExistsAsync(key);
        }

        public T Get<T>(string key)
        {
            var val = Database.StringGet(key);
            if (val.HasValue)
            {
                return (T)EntityConvertor.ToEntry(val,typeof(T));
            }
            return default;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var val = await Database.StringGetAsync(key);
            if (val.HasValue)
            {
                return (T)EntityConvertor.ToEntry(val, typeof(T));
            }
            return default;
        }

        public string GetString(string key)
        {
            return Database.StringGet(key);
        }

        public async Task<string> GetStringAsync(string key)
        {
            var res = await Database.StringGetAsync(key);
            return res;
        }

        public bool Set<T>(string key, T value, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var buffer = EntityConvertor.ToBytes(value, typeof(T));
            return Database.StringSet(key, buffer, cacheTime, (When)cacheSetIf, CommandFlags.None);
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var buffer = EntityConvertor.ToBytes(value, typeof(T));
            return Database.StringSetAsync(key, buffer, cacheTime, (When)cacheSetIf, CommandFlags.None);
        }

        public bool SetString(string key, string value, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            return Database.StringSet(key, value, cacheTime, (When)cacheSetIf, CommandFlags.None);
        }

        public Task<bool> SetStringAsync(string key, string value, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            return Database.StringSetAsync(key, value, cacheTime, (When)cacheSetIf, CommandFlags.None);
        }

        public bool Expire(string key, TimeSpan? cacheTime)
        {
            return Database.KeyExpire(key, cacheTime);
        }
        public Task<bool> ExpireAsync(string key, TimeSpan? cacheTime)
        {
            return Database.KeyExpireAsync(key, cacheTime);
        }

    }
}
