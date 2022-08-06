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
        public AutoCacheOptionsAttribute(bool canRenewal, string cacheTimeStr)
        {
            CanRenewal = canRenewal;
            CacheTime = TimeSpan.Parse(cacheTimeStr);
        }
        public AutoCacheOptionsAttribute(string cacheTimeStr)
        {
            CacheTime = TimeSpan.Parse(cacheTimeStr);
        }
        public AutoCacheOptionsAttribute(bool canRenewal)
        {
            CanRenewal = canRenewal;
        }
        public bool CanRenewal { get; }

        public TimeSpan CacheTime { get; }
    }
}
