using StackExchange.Redis;
namespace Ao.Cache.Redis
{
    public interface IAutoWriteCache
    {
        object Write(HashEntry[] entries);
    }
}
