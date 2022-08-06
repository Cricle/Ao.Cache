using StackExchange.Redis;

namespace Ao.Cache.InRedis.HashList
{
    public static class CacheOperatorWriteExtensions
    {
        public static void Write<T>(this IHashCacheOperator @operator, ref T instance, HashEntry[] entries)
        {
            object val = instance;
            @operator.Write(ref val, entries);
            instance = (T)val;
        }
        public static void Write<T>(this IListCacheOperator @operator, ref T instance, RedisValue[] entries)
        {
            object val = instance;
            @operator.Write(ref val, entries);
            instance = (T)val;
        }
    }
}
