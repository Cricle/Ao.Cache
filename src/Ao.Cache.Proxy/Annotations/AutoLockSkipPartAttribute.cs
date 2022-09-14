using System;

namespace Ao.Cache.Proxy.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class AutoLockSkipPartAttribute : KeySkipPartAttribute
    {

    }
}
