using Ao.Cache.Proxy.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy
{
    public static class CacheNamedHelperUnwindExtensions
    {
        class ExpressionCallCache
        {
            private static readonly Dictionary<int, WeakReference<Delegate>> expressionMap = new Dictionary<int, WeakReference<Delegate>>();
            private static readonly object syncRoot = new object();
            public static Delegate Get(Expression expression)
            {
                var h = ExpressionHasher.GetHashCode(expression);
                if (!expressionMap.TryGetValue(h, out var dele) ||
                    !dele.TryGetTarget(out var d))
                {
                    lock (syncRoot)
                    {
                        if (!expressionMap.TryGetValue(h, out dele))
                        {
                            d = Expression.Lambda(expression).Compile();
                            dele = new WeakReference<Delegate>(d);
                            expressionMap[h] = dele;
                        }
                        else if (!dele.TryGetTarget(out d))
                        {
                            dele.SetTarget(Expression.Lambda(expression).Compile());
                        }
                    }
                }
                return d;
            }
        }

        private static Type GetAcutalType<T>(object instance)
        {
            var type = typeof(T);
            if (type.IsInterface)
            {
                var fi = CastleTypeHelper.GetActualFieldInfo(instance.GetType());
                if (fi != null)
                {
                    type = fi.GetValue(instance).GetType();
                }
            }
            return type;
        }
        public static Task<bool> ExistsAsync<T, TEntity>(this AutoCacheService service, object instance, Expression<Func<T, Task<TEntity>>> call, bool cacheCall = true)
        {
            var type = GetAcutalType<T>(instance);
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.ExistsAsync(winObj);
        }
        public static Task<bool> SetInCacheAsync<T, TEntity>(this AutoCacheService service, object instance, TEntity entity, Expression<Func<T, Task<TEntity>>> call, bool cacheCall = true)
        {
            var type = GetAcutalType<T>(instance);
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.SetInCacheAsync(winObj, entity);
        }
        public static Task<bool> DeleteAsync<T, TEntity>(this AutoCacheService service, object instance, Expression<Func<T, Task<TEntity>>> call, bool cacheCall = true)
        {
            var type = GetAcutalType<T>(instance);
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.DeleteAsync(winObj);
        }
        public static Task<bool> RenewalAsync<T, TEntity>(this AutoCacheService service, object instance, TimeSpan? cacheTime, Expression<Func<T, Task<TEntity>>> call, bool cacheCall = true)
        {
            var type = GetAcutalType<T>(instance);
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.RenewalAsync(winObj, cacheTime);
        }


        public static Task<bool> ExistsAsync<T, TEntity>(this AutoCacheService service, Type type, Expression<Func<T, Task>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.ExistsAsync(winObj);
        }
        public static Task<bool> SetInCacheAsync<T, TEntity>(this AutoCacheService service, Type type, TEntity entity, Expression<Func<T, Task>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.SetInCacheAsync(winObj, entity);
        }
        public static Task<bool> DeleteAsync<T, TEntity>(this AutoCacheService service, Type type, Expression<Func<T, Task>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.DeleteAsync(winObj);
        }
        public static Task<bool> RenewalAsync<T, TEntity>(this AutoCacheService service, Type type, TimeSpan? cacheTime, Expression<Func<T, Task>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.RenewalAsync(winObj, cacheTime);
        }

        public static Task<bool> ExistsAsync<T, TEntity>(this AutoCacheService service, Type type, Expression<Action<T>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.ExistsAsync(winObj);
        }
        public static Task<bool> SetInCacheAsync<T, TEntity>(this AutoCacheService service, Type type, TEntity entity, Expression<Action<T>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.SetInCacheAsync(winObj, entity);
        }
        public static Task<bool> DeleteAsync<T, TEntity>(this AutoCacheService service, Type type, Expression<Action<T>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.DeleteAsync(winObj);
        }
        public static Task<bool> RenewalAsync<T, TEntity>(this AutoCacheService service, Type type, TimeSpan? cacheTime, Expression<Action<T>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(type, call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.RenewalAsync(winObj, cacheTime);
        }

        public static Task<bool> ExistsAsync<T, TEntity>(this AutoCacheService service, Expression<Func<T, Task<TEntity>>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.ExistsAsync(winObj);
        }
        public static Task<bool> SetInCacheAsync<T, TEntity>(this AutoCacheService service, TEntity entity, Expression<Func<T, Task<TEntity>>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.SetInCacheAsync(winObj, entity);
        }
        public static Task<bool> DeleteAsync<T, TEntity>(this AutoCacheService service, Expression<Func<T, Task<TEntity>>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.DeleteAsync(winObj);
        }
        public static Task<bool> RenewalAsync<T, TEntity>(this AutoCacheService service, TimeSpan? cacheTime, Expression<Func<T, Task<TEntity>>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.RenewalAsync(winObj, cacheTime);
        }


        public static Task<bool> ExistsAsync<T, TEntity>(this AutoCacheService service, Expression<Func<T, Task>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.ExistsAsync(winObj);
        }
        public static Task<bool> SetInCacheAsync<T, TEntity>(this AutoCacheService service, TEntity entity, Expression<Func<T, Task>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.SetInCacheAsync(winObj, entity);
        }
        public static Task<bool> DeleteAsync<T, TEntity>(this AutoCacheService service, Expression<Func<T, Task>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.DeleteAsync(winObj);
        }
        public static Task<bool> RenewalAsync<T, TEntity>(this AutoCacheService service, TimeSpan? cacheTime, Expression<Func<T, Task>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.RenewalAsync(winObj, cacheTime);
        }

        public static Task<bool> ExistsAsync<T, TEntity>(this AutoCacheService service, Expression<Action<T>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.ExistsAsync(winObj);
        }
        public static Task<bool> SetInCacheAsync<T, TEntity>(this AutoCacheService service, TEntity entity, Expression<Action<T>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.SetInCacheAsync(winObj, entity);
        }
        public static Task<bool> DeleteAsync<T, TEntity>(this AutoCacheService service, Expression<Action<T>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.DeleteAsync(winObj);
        }
        public static Task<bool> RenewalAsync<T, TEntity>(this AutoCacheService service, TimeSpan? cacheTime, Expression<Action<T>> call, bool cacheCall = true)
        {
            var winObj = service.NamedHelper.GetUnwindObject(call, cacheCall);
            var finder = service.GetEmpty<TEntity>();
            return finder.RenewalAsync(winObj, cacheTime);
        }
        public static UnwindObject GetUnwindObject<T, TEntity>(this AutoCacheService service, Expression<Action<T>> call, bool cacheCall = true)
        {
            return service.NamedHelper.GetUnwindObject(call, cacheCall);
        }
        private static UnwindObject GetUnwindObject(Type tt, ICacheNamedHelper helper, MethodCallExpression exp, bool cacheCall = true)
        {
            var vals = helper.GetArgIndexs(
                new NamedInterceptorKey(tt, exp.Method));
            var args = new object[vals.ArgIndexs == null ? exp.Arguments.Count : vals.ArgIndexs.Count];
            var argsIndex = 0;
            for (int i = 0; i < exp.Arguments.Count; i++)
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
                else
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
            }
            return helper.GetUnwindObject(new NamedInterceptorKey(tt, exp.Method), args, true);
        }
        public static UnwindObject GetUnwindObject<T>(this ICacheNamedHelper helper, Type type, Expression<Action<T>> call, bool cacheCall = true)
        {
            if (call.Body is MethodCallExpression exp)
            {
                return GetUnwindObject(type, helper, exp, cacheCall);
            }
            throw new NotSupportedException(call.Body.ToString());
        }
        public static UnwindObject GetUnwindObject<T, TResult>(this ICacheNamedHelper helper, Type type, Expression<Func<T, Task<TResult>>> call, bool cacheCall = true)
        {
            if (call.Body is MethodCallExpression exp)
            {
                return GetUnwindObject(type, helper, exp, cacheCall);
            }
            throw new NotSupportedException(call.Body.ToString());
        }
        public static UnwindObject GetUnwindObject<T>(this ICacheNamedHelper helper, Type type, Expression<Func<T, Task>> call, bool cacheCall = true)
        {
            if (call.Body is MethodCallExpression exp)
            {
                return GetUnwindObject(type, helper, exp, cacheCall);
            }
            throw new NotSupportedException(call.Body.ToString());
        }
        public static UnwindObject GetUnwindObject<T>(this ICacheNamedHelper helper, Expression<Action<T>> call, bool cacheCall = true)
        {
            if (call.Body is MethodCallExpression exp)
            {
                return GetUnwindObject(typeof(T), helper, exp, cacheCall);
            }
            throw new NotSupportedException(call.Body.ToString());
        }
        public static UnwindObject GetUnwindObject<T>(this ICacheNamedHelper helper, Expression<Func<T, Task>> call, bool cacheCall = true)
        {
            if (call.Body is MethodCallExpression exp)
            {
                return GetUnwindObject(typeof(T), helper, exp, cacheCall);
            }
            throw new NotSupportedException(call.Body.ToString());
        }
        public static UnwindObject GetUnwindObject<T, TResult>(this ICacheNamedHelper helper, Expression<Func<T, Task<TResult>>> call, bool cacheCall = true)
        {
            if (call.Body is MethodCallExpression exp)
            {
                return GetUnwindObject(typeof(T), helper, exp, cacheCall);
            }
            throw new NotSupportedException(call.Body.ToString());
        }


        public static Task<bool> ExistsAsync<TEntity>(this AutoCacheService service, Type targetType, string methodName, params object[] args)
        {
            var winObj = service.NamedHelper.GetUnwindObject(targetType, methodName, args);
            var finder = service.GetEmpty<TEntity>();
            return finder.ExistsAsync(winObj);
        }
        public static Task<bool> SetInCacheAsync<TEntity>(this AutoCacheService service, TEntity entity, Type targetType, string methodName, params object[] args)
        {
            var winObj = service.NamedHelper.GetUnwindObject(targetType, methodName, args);
            var finder = service.GetEmpty<TEntity>();
            return finder.SetInCacheAsync(winObj, entity);
        }
        public static Task<bool> DeleteAsync<TEntity>(this AutoCacheService service, Type targetType, string methodName, params object[] args)
        {
            var winObj = service.NamedHelper.GetUnwindObject(targetType, methodName, args);
            var finder = service.GetEmpty<TEntity>();
            return finder.DeleteAsync(winObj);
        }
        public static Task<bool> RenewalAsync<TEntity>(this AutoCacheService service, TimeSpan? cacheTime, Type targetType, string methodName, params object[] args)
        {
            var winObj = service.NamedHelper.GetUnwindObject(targetType, methodName, args);
            var finder = service.GetEmpty<TEntity>();
            return finder.RenewalAsync(winObj, cacheTime);
        }

        public static UnwindObject GetUnwindObject<TEntity>(this AutoCacheService service, Type targetType, string methodName, params object[] args)
        {
            return service.NamedHelper.GetUnwindObject(targetType, methodName, args);
        }
        public static UnwindObject GetUnwindObject(this ICacheNamedHelper helper, Type targetType, string methodName, params object[] args)
        {
            return helper.GetUnwindObject(new NamedInterceptorKey(targetType, targetType.GetMethod(methodName)), args);
        }
    }
}