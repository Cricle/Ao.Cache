using StackExchange.Redis;

namespace Ao.Cache.InRedis.HashList
{
    public interface IListCacheOperator: ICacheOperator<RedisValue[]>
    {
    }
}
