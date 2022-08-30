using System;

namespace Ao.Cache.CastleProxy.Model
{
    [Flags]
    public enum AutoCacheStatus
    {
        Skip = 0,
        MethodHit = 1,
        CacheHit = 2,
        Intercept = 3,
    }
}
