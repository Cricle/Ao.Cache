using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public partial interface ICacheVisitor
    {
        IDictionary<string, bool> Exists(IReadOnlyList<string> keys);

        Task<IDictionary<string, bool>> ExistsAsync(IReadOnlyList<string> keys);

        IDictionary<string, T> Get<T>(IReadOnlyList<string> keys);

        Task<IDictionary<string, T>> GetAsync<T>(IReadOnlyList<string> keys);

        IDictionary<string, bool> Set<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        Task<IDictionary<string, bool>> SetAsync<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        IDictionary<string, string> GetString(IReadOnlyList<string> keys);

        Task<IDictionary<string, string>> GetStringAsync(IReadOnlyList<string> keys);

        IDictionary<string, bool> SetString(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        Task<IDictionary<string, bool>> SetStringAsync(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always);

        IDictionary<string, bool> Delete(IReadOnlyList<string> keys);

        Task<IDictionary<string, bool>> DeleteAsync(IReadOnlyList<string> keys);

        IDictionary<string, bool> Expire(IReadOnlyList<string> keys, TimeSpan? cacheTime);

        Task<IDictionary<string, bool>> ExpireAsync(IReadOnlyList<string> keys, TimeSpan? cacheTime);
    }
}
