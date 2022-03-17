using StackExchange.Redis;

namespace Ao.Cache.Redis
{
    public interface IListCacheOperator: ICacheOperator<RedisValue[]>
    {
    }
}
