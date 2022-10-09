namespace Ao.Cache.Proxy.Model
{
    public interface IAutoCacheResult
    {
        object RawData { get; set; }

        AutoCacheStatus Status { get; set; }
    }
}
