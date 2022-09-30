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

            public Action<object, object> GetRawAndSet; 
        }
        private static readonly Dictionary<Type, PropertyProxy> proxy = new Dictionary<Type, PropertyProxy>();
        private static readonly object syncRoot = new object();

        public static object GetRawResult(object instance, Type type)
        {
            var func = GetRawResultDelegate(type);
            return func.Getter(instance);
        }
        public static void SetRawResult(object instance, object value, Type type)
        {
            var func = GetRawResultDelegate(type);
            func.Setter(instance, value);
        }
        public static void SetAndSetRawResult(object provider, object source, Type type)
        {
            var func = GetRawResultDelegate(type);
            func.GetRawAndSet(provider, source);
        }

        private static Func<object, object> CompileGetter(Type type)
        {
            var actType = typeof(AutoCacheResult<>)
                            .MakeGenericType(type);
            var par1 = Expression.Parameter(typeof(object));
            var par1Cast = Expression.Convert(par1, actType);
            var prop = actType.GetProperty(nameof(AutoCacheResult<object>.RawData));
            var caller = Expression.Convert(Expression.Call(par1Cast, prop.GetMethod), typeof(object));
            return Expression.Lambda<Func<object, object>>(caller, par1)
                .Compile();
        }
        private static Action<object, object> CompileSetter(Type type)
        {
            var actType = typeof(AutoCacheResult<>)
                .MakeGenericType(type);
            var spar1 = Expression.Parameter(typeof(object));
            var spar2 = Expression.Parameter(typeof(object));
            var spar1Cast = Expression.Convert(spar1, actType);
            var spar2Cast = Expression.Convert(spar2, type);
            var prop = actType.GetProperty(nameof(AutoCacheResult<object>.RawData));
            return Expression.Lambda<Action<object, object>>(Expression.Call(spar1Cast, prop.SetMethod, spar2Cast), spar1, spar2)
                .Compile();
        }
        private static Action<object, object> CompileGetRawAndSet(Type type)
        {
            var actType = typeof(AutoCacheResult<>).MakeGenericType(type);
            var par1 = Expression.Parameter(typeof(object));
            var par2 = Expression.Parameter(typeof(object));

            var castPar1 = Expression.Convert(par1, actType);
            var castPar2 = Expression.Convert(par2, actType);

            var prop = actType.GetProperty(nameof(AutoCacheResult<object>.RawData));
            var getAndSet = Expression.Call(castPar2,
                prop.SetMethod,
                Expression.Call(castPar1, prop.GetMethod));

            return Expression.Lambda<Action<object, object>>(getAndSet, par1, par2).Compile();
        }
        private static PropertyProxy GetRawResultDelegate(Type type)
        {
            if (!proxy.TryGetValue(type, out var func))
            {
                lock (syncRoot)
                {
                    if (!proxy.TryGetValue(type, out func))
                    {
                        var getter = CompileGetter(type);
                        var setter = CompileSetter(type);
                        var getAndSet = CompileGetRawAndSet(type);

                        func = new PropertyProxy
                        {
                            Getter = getter,
                            Setter = setter,
                            GetRawAndSet = getAndSet
                        };
                        proxy[type] = func;
                    }
                }
            }
            return func;
        }
    }
}
