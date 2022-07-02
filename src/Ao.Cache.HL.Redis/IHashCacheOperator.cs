using StackExchange.Redis;

namespace Ao.Cache.HL.Redis
{
    public interface IHashCacheOperator : ICacheOperator<HashEntry[]>
    {
    }
}
