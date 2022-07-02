using StackExchange.Redis;
namespace Ao.Cache.InRedis.HashList
{
    public interface IAutoWriteCache
    {
        object Write(HashEntry[] entries);
    }
}
