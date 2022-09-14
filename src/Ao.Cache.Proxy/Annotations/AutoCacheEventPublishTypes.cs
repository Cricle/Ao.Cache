using System;

namespace Ao.Cache.Proxy.Annotations
{
    [Flags]
    public enum AutoCacheEventPublishTypes
    {
        MethodFound = 1,
        CacheFound = MethodFound << 1
    }
}
