using System;

namespace Ao.Cache.Core.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CacheProxyMethodAttribute: Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class CacheProxyAttribute : Attribute
    {
        public Type ProxyType { get; set; }

        public string EndName { get; set; }
    }
}
