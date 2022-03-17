using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Redis
{
    public static class RedisKeyScanExtensions
    {
        public const int DefaultPageSize = 200;

        public static Task<long> DeleteScanKeysAsync(this IDatabase database, string pattern)
        {
            return DeleteScanKeysAsync(database, pattern, DefaultPageSize);
        }
        public static async Task<long> DeleteScanKeysAsync(this IDatabase database, string pattern, int pageSize)
        {
            var count = 0L;
            await foreach (var item in ScanKeys(database, pattern, pageSize))
            {
                var keys = AsRedisKey(item);
                count += await database.KeyDeleteAsync(keys);
            }
            return count;
        }
        public static IAsyncEnumerable<string[]> ScanKeys(this IDatabase database, string pattern)
        {
            return ScanKeys(database, pattern, DefaultPageSize);
        }
        public static async IAsyncEnumerable<string[]> ScanKeys(this IDatabase database, string pattern, int pageSize)
        {
            var count = 0L;
            do
            {
                var res = await database.ExecuteAsync("scan", count, "match", pattern, "count", pageSize);
                var f = ((RedisResult[])res);
                count = ((long)f[0]);
                yield return ((string[])f[1]);
            } while (count != 0);
        }
        private static RedisKey[] AsRedisKey(string[] keys)
        {
#if NET5_0_OR_GREATER
            var val = GC.AllocateUninitializedArray<RedisKey>(keys.Length);
#else
            var val = new RedisKey[keys.Length];
#endif
            for (int i = 0; i < keys.Length; i++)
            {
                val[i] = keys[i];
            }
            return val;
        }
    }
}
