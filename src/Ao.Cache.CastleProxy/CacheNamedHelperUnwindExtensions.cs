using Ao.Cache.CastleProxy.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ao.Cache.CastleProxy
{
    public static class CacheNamedHelperUnwindExtensions
    {
        class ExpressionCallCache
        {
            private static readonly Dictionary<int, Delegate> expressionMap = new Dictionary<int, Delegate>();
            private static readonly object syncRoot = new object();
            public static Delegate Get(Expression expression)
            {
                var h = ExpressionHasher.GetHashCode(expression);
                if (!expressionMap.TryGetValue(h, out var dele))
                {
                    lock (syncRoot)
                    {
                        if (!expressionMap.TryGetValue(h, out dele))
                        {
                            dele = Expression.Lambda(expression).Compile();
                            expressionMap[h] = dele;
                        }
                    }
                }
                return dele;
            }
        }
        public static UnwindObject GetUnwindObject<T>(this ICacheNamedHelper helper, Expression<Action<T>> call, bool cacheCall = false)
        {
            return GetUnwindObject(helper, DefaultCacheNamedHelper.Default, call, cacheCall);
        }
        public static UnwindObject GetUnwindObject<T>(this ICacheNamedHelper helper, ICacheNamedHelper namedHelper, Expression<Action<T>> call,bool cacheCall=false)
        {
            if (call.Body is MethodCallExpression exp)
            {
                var tt = typeof(T);
                var vals = namedHelper.GetArgIndexs(
                    new NamedInterceptorKey(tt, exp.Method));
                var args = new object[vals.ArgIndexs == null ? exp.Arguments.Count : vals.ArgIndexs.Count];
                var argsIndex = 0;
                for (int i = 0; i < exp.Arguments.Count; i++, argsIndex++)
                {
                    if (vals.ArgIndexs != null && !vals.ArgIndexs.Contains(i))
                    {
                        continue;
                    }
                    var arg = exp.Arguments[i];
                    if (arg is ConstantExpression constExp)
                    {
                        args[argsIndex++] = constExp.Value;
                    }
                    else if (arg is UnaryExpression ||arg is MethodCallExpression||arg is MemberExpression)
                    {
                        if (cacheCall)
                        {
                            var res = ExpressionCallCache.Get(arg).DynamicInvoke();
                            args[argsIndex++] = res;
                        }
                        else
                        {
                            var res = Expression.Lambda(arg).Compile().DynamicInvoke();
                            args[argsIndex++] = res;
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"No support {arg.ToString()}");
                    }
                }
                return helper.GetUnwindObject(new NamedInterceptorKey(tt, exp.Method), args);
            }
            throw new NotSupportedException(call.Body.ToString());
        }
        public static UnwindObject GetUnwindObject<T>(this ICacheNamedHelper helper, string methodName, params object[] args)
        {
            return GetUnwindObject(helper, typeof(T), methodName, args);
        }
        public static UnwindObject GetUnwindObject(this ICacheNamedHelper helper, Type targetType, string methodName, params object[] args)
        {
            return helper.GetUnwindObject(new NamedInterceptorKey(targetType, targetType.GetMethod(methodName)), args);
        }
    }
}