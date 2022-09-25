using Ao.Cache.Proxy;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy
{
    public static class CastleCacheNamedHelperUnwindExtensions
    {
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

    }
}
