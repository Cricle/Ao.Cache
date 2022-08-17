using Ao.Cache.CastleProxy.Annotations;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.Cache.CastleProxy.Interceptors
{
    static class AutoLockAttrCache
    {
        private static readonly object locker = new object();
        private static readonly Dictionary<NamedInterceptorKey, AutoLockAttribute> attributes = new Dictionary<NamedInterceptorKey, AutoLockAttribute>();

        public static AutoLockAttribute Get(in NamedInterceptorKey key)
        {
            if (!attributes.TryGetValue(key, out var attr))
            {
                lock (locker)
                {
                    if (!attributes.TryGetValue(key, out attr))
                    {
                        attr = key.TargetType.GetCustomAttribute<AutoLockAttribute>() ??
                            key.Method.GetCustomAttribute<AutoLockAttribute>();
                        attributes[key] = attr;
                    }
                }
            }
            return attr;
        }
    }
}
