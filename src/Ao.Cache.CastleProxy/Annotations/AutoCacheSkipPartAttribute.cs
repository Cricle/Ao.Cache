using System;

namespace Ao.Cache.CastleProxy.Annotations
{
    public class KeySkipPartAttribute : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class AutoCacheSkipPartAttribute: KeySkipPartAttribute
    {

    }
}
