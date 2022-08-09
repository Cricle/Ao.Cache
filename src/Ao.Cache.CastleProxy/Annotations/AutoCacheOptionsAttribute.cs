using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AutoCacheOptionsAttribute : Attribute
    {
        public static readonly TimeSpan? DefaultCacheTime = TimeSpan.FromSeconds(3);

        public AutoCacheOptionsAttribute()
        {
        }

        public AutoCacheOptionsAttribute(string cacheTimeStr)
        {
            CacheTime = TimeSpan.Parse(cacheTimeStr);
        }

        public bool CanRenewal { get; set; }

        public TimeSpan? CacheTime { get; } = DefaultCacheTime;

        public bool Renewal { get; set; }
    }
}
