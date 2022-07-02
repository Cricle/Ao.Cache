using StackExchange.Redis;

namespace Ao.Cache.InRedis.HashList
{
    public interface IEntryCacheOperator
    {
        void Build();

        void Write(ref object instance, RedisValue entry);

        RedisValue As(object value);
    }
}
