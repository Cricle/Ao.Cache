using StackExchange.Redis;

namespace Ao.Cache.InRedis.HashList
{
    public interface IHashCacheOperator : ICacheOperator<HashEntry[]>
    {
    }
}
