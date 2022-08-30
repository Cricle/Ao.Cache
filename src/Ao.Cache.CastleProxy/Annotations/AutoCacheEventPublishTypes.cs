using System;

namespace Ao.Cache.CastleProxy.Annotations
{
    [Flags]
    public enum AutoCacheEventPublishTypes
    {
        MethodFound = 1,
        CacheFound = MethodFound << 1
    }
}
