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
                count += await database.KeyDeleteAsync(item);
            }
            return count;
        }
        public static IAsyncEnumerable<RedisKey[]> ScanKeys(this IDatabase database, string pattern)
        {
            return ScanKeys(database, pattern, DefaultPageSize);
        }
        public static async IAsyncEnumerable<RedisKey[]> ScanKeys(this IDatabase database, string pattern, int pageSize)
        {
            var count = 0L;
            do
            {
                var res = await database.ExecuteAsync("scan", count, "match", pattern, "count", pageSize);
                var f = ((RedisResult[])res);
                count = ((long)f[0]);
                yield return ((RedisKey[])f[1]);
            } while (count != 0);
        }
    }
}
