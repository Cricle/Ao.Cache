using System;

namespace Ao.Cache.CastleProxy.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class AutoLockSkipPartAttribute : KeySkipPartAttribute
    {

    }
}
