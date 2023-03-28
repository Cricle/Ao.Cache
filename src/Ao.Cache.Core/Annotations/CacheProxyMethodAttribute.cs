using System;

namespace Ao.Cache.Core.Annotations
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CacheProxyMethodAttribute : Attribute
    {
        public string CacheTime { get; set; }

        public bool Renewal { get; set; }

        public bool Inline { get; set; } = true;

        public bool NoProxy { get; set; }

        public string Head { get; set; }

        public bool HeadAbsolute { get; set; }
    }
}
