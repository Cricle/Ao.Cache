using System;

namespace Ao.Cache.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CacheForDataAccesstorAttribute : Attribute
    {
        public Type ServiceType { get; set; }

        public Type ImplementType { get; set; }
    }
}
