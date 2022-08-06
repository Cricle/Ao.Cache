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
        public AutoCacheOptionsAttribute()
        {
        }

        public AutoCacheOptionsAttribute(string cacheTimeStr)
        {
            CacheTime = TimeSpan.Parse(cacheTimeStr);
        }

        public bool CanRenewal { get; set; }

        public TimeSpan? CacheTime { get; }

        public bool Renewal { get; set; }
    }
}
