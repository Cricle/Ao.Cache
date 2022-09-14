using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Annotations
{
    public static class AutoCacheAssertions
    {
        private static readonly object hasAutoCacheLocker = new object();
        private static readonly Dictionary<MemberInfo, bool> hasAutoCache = new Dictionary<MemberInfo, bool>();

        public static bool HasAutoCache(MemberInfo info)
        {
            if (!hasAutoCache.TryGetValue(info, out var b))
            {
                lock (hasAutoCacheLocker)
                {
                    if (!hasAutoCache.TryGetValue(info, out b))
                    {
                        b = info.GetCustomAttribute<AutoCacheAttribute>() != null;
                        hasAutoCache[info] = b;
                    }
                }
            }
            return b;
        }
    }
}
