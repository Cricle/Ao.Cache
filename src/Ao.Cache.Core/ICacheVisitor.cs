using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public partial interface ICacheVisitor
    {
        bool Exists(string key);

        Task<bool> ExistsAsync(string key);

        T Get<T>(string key);

        Task<T> GetAsync<T>(string key);

        bool Set<T>(string key, T value, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        string GetString(string key);

        Task<string> GetStringAsync(string key);

        bool SetString(string key, string value, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        Task<bool> SetStringAsync(string key, string value, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        bool Delete(string key);

        Task<bool> DeleteAsync(string key);

        bool Expire(string key, TimeSpan? cacheTime);

        Task<bool> ExpireAsync(string key, TimeSpan? cacheTime);

    }
}
