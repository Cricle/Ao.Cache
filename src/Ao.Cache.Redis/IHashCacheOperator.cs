using StackExchange.Redis;

namespace Ao.Cache.Redis
{
    public interface IHashCacheOperator : ICacheOperator<HashEntry[]>
    {
    }
}
