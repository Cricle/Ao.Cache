using System;
using System.Collections.Generic;
using System.Text;

namespace Ao.Cache.Annotations
{
    public abstract class CacheProxyByAttribute : Attribute
    {
        public Type ProxyType { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CacheProxyByInterfaceAttribute : CacheProxyByAttribute
    {

    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CacheProxyByClassAttribute : CacheProxyByAttribute
    {

    }
}
