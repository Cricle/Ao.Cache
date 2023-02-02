namespace Ao.Cache.Proxy.Model
{
    public class AutoCacheResult<T> : IAutoCacheResult
    {
        public T RawData { get; set; }

        public AutoCacheStatus Status { get; set; }

        object IAutoCacheResult.RawData { get => RawData; set => RawData = (T)value; }
    }
}
