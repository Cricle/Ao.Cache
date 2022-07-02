using StackExchange.Redis;

namespace Ao.Cache.HL.Redis
{
    public interface IListCacheOperator: ICacheOperator<RedisValue[]>
    {
    }
}
