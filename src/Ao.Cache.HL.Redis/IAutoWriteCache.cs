using StackExchange.Redis;
namespace Ao.Cache.HL.Redis
{
    public interface IAutoWriteCache
    {
        object Write(HashEntry[] entries);
    }
}
