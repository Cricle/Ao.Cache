using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.InMemory
{
    public partial class MemoryCacheVisitor
    {
        public IDictionary<string, bool> Delete(IReadOnlyList<string> keys)
        {
            var map = new Dictionary<string, bool>(keys.Count);
            for (int i = 0; i < keys.Count; i++)
            {
                var data = keys[i];
                map[data] = Delete(data);
            }
            return map;
        }

        public Task<IDictionary<string, bool>> DeleteAsync(IReadOnlyList<string> keys)
        {
            return Task.FromResult(Delete(keys));
        }

        public IDictionary<string, bool> Exists(IReadOnlyList<string> keys)
        {
            var map = new Dictionary<string, bool>(keys.Count);
            for (int i = 0; i < keys.Count; i++)
            {
                var data = keys[i];
                map[data] = Exists(data);
            }
            return map;
        }

        public Task<IDictionary<string, bool>> ExistsAsync(IReadOnlyList<string> keys)
        {
            return Task.FromResult(Exists(keys));
        }

        public IDictionary<string, bool> Expire(IReadOnlyList<string> keys, TimeSpan? cacheTime)
        {
            var map = new Dictionary<string, bool>(keys.Count);
            for (int i = 0; i < keys.Count; i++)
            {
                var data = keys[i];
                map[data] = Expire(data, cacheTime);
            }
            return map;
        }

        public Task<IDictionary<string, bool>> ExpireAsync(IReadOnlyList<string> keys, TimeSpan? cacheTime)
        {
            return Task.FromResult(Expire(keys, cacheTime));
        }

        public IDictionary<string, T> Get<T>(IReadOnlyList<string> keys)
        {
            var map = new Dictionary<string, T>(keys.Count);
            for (int i = 0; i < keys.Count; i++)
            {
                var data = keys[i];
                map[data] = Get<T>(data);
            }
            return map;
        }

        public Task<IDictionary<string, T>> GetAsync<T>(IReadOnlyList<string> keys)
        {
            return Task.FromResult(Get<T>(keys));
        }

        public IDictionary<string, string> GetString(IReadOnlyList<string> keys)
        {
            var map = new Dictionary<string, string>(keys.Count);
            for (int i = 0; i < keys.Count; i++)
            {
                var data = keys[i];
                map[data] = GetString(data);
            }
            return map;
        }

        public Task<IDictionary<string, string>> GetStringAsync(IReadOnlyList<string> keys)
        {
            return Task.FromResult(GetString(keys));
        }

        public IDictionary<string, bool> Set<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var map = new Dictionary<string, bool>(datas.Length);
            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                map[data.Key] = Set(data.Key, data.Value, cacheTime, cacheSetIf);
            }
            return map;
        }

        public Task<IDictionary<string, bool>> SetAsync<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            return Task.FromResult(Set(datas, cacheTime, cacheSetIf));
        }

        public IDictionary<string, bool> SetString(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var map = new Dictionary<string, bool>(datas.Length);
            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                map[data.Key] = SetString(data.Key, data.Value, cacheTime, cacheSetIf);
            }
            return map;
        }

        public Task<IDictionary<string, bool>> SetStringAsync(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            return Task.FromResult(SetString(datas, cacheTime, cacheSetIf));
        }
    }
}
