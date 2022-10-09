namespace Ao.Cache.Proxy.Model
{
    public sealed class AutoCacheResult<T> : IAutoCacheResult
    {
        public T RawData { get; set; }

        public AutoCacheStatus Status { get; set; }

        object IAutoCacheResult.RawData { get => RawData; set => RawData = (T)value; }
    }
}
