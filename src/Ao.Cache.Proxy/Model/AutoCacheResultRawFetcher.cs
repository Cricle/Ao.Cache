using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ao.Cache.Proxy.Model
{
    public static class AutoCacheResultRawFetcher
    {
        class PropertyProxy
        {
            public Func<object, object> Getter;

            public Action<object, object> Setter;
        }
        private static readonly Dictionary<Type, PropertyProxy> proxy = new Dictionary<Type, PropertyProxy>();
        private static readonly object syncRoot = new object();
        
        public static object GetRawResult(object instance,Type type)
        {
            var func = GetRawResultDelegate(type);
            return func.Getter(instance);
        }
        public static void SetRawResult(object instance,object value, Type type)
        {
            var func = GetRawResultDelegate(type);
            func.Setter(instance,value);
        }
        private static PropertyProxy GetRawResultDelegate(Type type)
        {
            if (!proxy.TryGetValue(type,out var func))
            {
                lock (syncRoot)
                {
                    if (!proxy.TryGetValue(type, out func))
                    {
                        var actType = typeof(AutoCacheResult<>)
                            .MakeGenericType(type);
                        var par1 = Expression.Parameter(typeof(object));
                        var par1Cast = Expression.Convert(par1, actType);
                        var prop = actType.GetProperty(nameof(AutoCacheResult<object>.RawData));
                        var getter = Expression.Lambda<Func<object, object>>(Expression.Call(par1Cast, prop.GetMethod),par1)
                            .Compile();

                        var spar1 = Expression.Parameter(typeof(object));
                        var spar2 = Expression.Parameter(typeof(object));
                        var spar1Cast = Expression.Convert(spar1, actType);
                        var spar2Cast = Expression.Convert(spar2, type);
                        var setter = Expression.Lambda<Action<object, object>>(Expression.Call(spar1Cast, prop.SetMethod, spar2Cast),spar1,spar2)
                            .Compile();
                        func = new PropertyProxy
                        {
                            Getter = getter,
                            Setter = setter
                        };
                        proxy[type] = func;
                    }
                }
            }
            return func;
        }
    }
}
