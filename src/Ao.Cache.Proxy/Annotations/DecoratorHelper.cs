using Ao.Cache.Proxy.Interceptors;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.Cache.Proxy.Annotations
{
    public static class DecoratorHelper
    {
        private static readonly Dictionary<NamedInterceptorKey, AutoCacheDecoratorBaseAttribute[]> m = new Dictionary<NamedInterceptorKey, AutoCacheDecoratorBaseAttribute[]>();
        private static readonly object locker = new object();

        public static AutoCacheDecoratorBaseAttribute[] Get(NamedInterceptorKey info)
        {
            if (!m.TryGetValue(info, out var attr))
            {
                lock (locker)
                {
                    if (!m.TryGetValue(info, out attr))
                    {
                        var attrs = new List<AutoCacheDecoratorBaseAttribute>();
                        var typeAttr = info.TargetType.GetCustomAttributes<AutoCacheDecoratorBaseAttribute>();
                        if (typeAttr != null)
                        {
                            attrs.AddRange(typeAttr);
                        }
                        var methodAttr = info.Method.GetCustomAttributes<AutoCacheDecoratorBaseAttribute>();
                        if (methodAttr != null)
                        {
                            attrs.AddRange(methodAttr);
                        }
                        attrs.Sort((a, b) => a.Order - b.Order);
                        attr = attrs.ToArray();
                        m[info] = attr;
                    }
                }
            }
            return attr;
        }
    }
}
