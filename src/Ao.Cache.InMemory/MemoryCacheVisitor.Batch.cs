using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InMemory
{
    public partial class MemoryCacheVisitor
    {
        public long Delete(IReadOnlyList<string> keys)
        {
            var res = 0;
            for (int i = 0; i < keys.Count; i++)
            {
                if (Delete(keys[i])) res++;
            }
            return res;
        }

        public Task<long> DeleteAsync(IReadOnlyList<string> keys)
        {
            return Task.FromResult(Delete(keys));
        }

        public long Exists(IReadOnlyList<string> keys)
        {
            var res = 0;
            for (int i = 0; i < keys.Count; i++)
            {
                if (Exists(keys[i])) res++;
            }
            return res;
        }

        public Task<long> ExistsAsync(IReadOnlyList<string> keys)
        {
            return Task.FromResult(Exists(keys));
        }

        public long Expire(IReadOnlyList<string> keys, TimeSpan? cacheTime)
        {
            var res = 0;
            for (int i = 0; i < keys.Count; i++)
            {
                if (Expire(keys[i],cacheTime)) res++;
            }
            return res;
        }

        public Task<long> ExpireAsync(IReadOnlyList<string> keys, TimeSpan? cacheTime)
        {
            return Task.FromResult(Expire(keys, cacheTime));
        }

        public IReadOnlyList<T> Get<T>(IReadOnlyList<string> keys)
        {
            var map = new T[keys.Count];
            for (int i = 0; i < keys.Count; i++)
            {
                map[i] = Get<T>(keys[i]);
            }
            return map;
        }

        public Task<IReadOnlyList<T>> GetAsync<T>(IReadOnlyList<string> keys)
        {
            return Task.FromResult(Get<T>(keys));
        }

        public IReadOnlyList<string> GetString(IReadOnlyList<string> keys)
        {
            var map = new string[keys.Count];
            for (int i = 0; i < keys.Count; i++)
            {
                map[i] = GetString(keys[i]);
            }
            return map;
        }

        public Task<IReadOnlyList<string>> GetStringAsync(IReadOnlyList<string> keys)
        {
            return Task.FromResult(GetString(keys));
        }

        public long Set<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var res = 0;
            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if (Set(data.Key, data.Value, cacheTime, cacheSetIf)) res++;
            }
            return res;
        }

        public Task<long> SetAsync<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            return Task.FromResult(Set(datas, cacheTime, cacheSetIf));
        }

        public long SetString(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var res = 0;
            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if (SetString(data.Key, data.Value, cacheTime, cacheSetIf)) res++;
            }
            return res;
        }

        public Task<long> SetStringAsync(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            return Task.FromResult(SetString(datas, cacheTime, cacheSetIf));
        }
    }
}
