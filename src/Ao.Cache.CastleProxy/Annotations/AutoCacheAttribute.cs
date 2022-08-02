using System;

namespace Ao.Cache.CastleProxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AutoCacheAttribute : Attribute
    {
    }
}
