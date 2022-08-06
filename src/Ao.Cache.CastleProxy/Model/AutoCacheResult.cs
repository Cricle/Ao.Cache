using System;

namespace Ao.Cache.CastleProxy.Model
{
    public interface IAutoCacheResult
    {
        AutoCacheStatus Status { get; set; }
    }

    public sealed class AutoCacheResult<T> : IAutoCacheResult
    {
        public T RawData { get; set; }

        public AutoCacheStatus Status { get; set; }
    }
    [Flags]
    public enum AutoCacheStatus
    {
        Skip = 0,
        MethodHit = 1,
        CacheHit = 2
    }
}
