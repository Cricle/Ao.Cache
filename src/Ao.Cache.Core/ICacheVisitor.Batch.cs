using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public partial interface ICacheVisitor
    {
        long Exists(IReadOnlyList<string> keys);

        Task<long> ExistsAsync(IReadOnlyList<string> keys);

        IReadOnlyList<T> Get<T>(IReadOnlyList<string> keys);

        Task<IReadOnlyList<T>> GetAsync<T>(IReadOnlyList<string> keys);

        long Set<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        Task<long> SetAsync<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        IReadOnlyList<string> GetString(IReadOnlyList<string> keys);

        Task<IReadOnlyList<string>> GetStringAsync(IReadOnlyList<string> keys);

        long SetString(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        Task<long> SetStringAsync(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        long Delete(IReadOnlyList<string> keys);

        Task<long> DeleteAsync(IReadOnlyList<string> keys);

        long Expire(IReadOnlyList<string> keys, TimeSpan? cacheTime);

        Task<long> ExpireAsync(IReadOnlyList<string> keys, TimeSpan? cacheTime);
    }
}
