namespace Ao.Cache.CastleProxy.Model
{
    public sealed class AutoCacheResult<T> : IAutoCacheResult
    {
        public T RawData { get; set; }

        public AutoCacheStatus Status { get; set; }
    }
}
