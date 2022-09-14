using System;

namespace Ao.Cache.Proxy.Model
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
