using System;
using System.Reflection;

namespace Ao.Cache.Core.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CacheProxyMethodAttribute: Attribute
    {
        public string CacheTime { get; set; }

        public bool Renewal { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class CacheProxyAttribute : Attribute
    {
        public Type ProxyType { get; set; }

        public string EndName { get; set; }

        public string NameSpace { get; set; }
    }
}
