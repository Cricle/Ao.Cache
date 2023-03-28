using System;
using System.Reflection;

namespace Ao.Cache.Core.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class CacheProxyAttribute : Attribute
    {
        public Type ProxyType { get; set; }

        public string EndName { get; set; }

        public string NameSpace { get; set; }

        public bool ProxyAll { get; set; }

        public string Head { get; set; }
    }
}
