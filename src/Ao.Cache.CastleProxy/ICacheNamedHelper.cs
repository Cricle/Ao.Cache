using Ao.Cache.CastleProxy.Interceptors;
using System;

namespace Ao.Cache.CastleProxy
{
    public static class CacheNamedHelperUnwindExtensions
    {
        public static UnwindObject GetUnwindObject<T>(this ICacheNamedHelper helper, string methodName, params object[] args)
        {
            return GetUnwindObject(helper, typeof(T), methodName, args);
        }
        public static UnwindObject GetUnwindObject(this ICacheNamedHelper helper, Type targetType, string methodName, params object[] args)
        {
            return helper.GetUnwindObject(new NamedInterceptorKey(targetType, targetType.GetMethod(methodName)), args);
        }
    }
    public interface ICacheNamedHelper
    {
        NamedInterceptorValue GetArgIndexs(in NamedInterceptorKey key);
        IStringTransfer GetStringTransfer(in NamedInterceptorKey key);
        UnwindObject GetUnwindObject(in NamedInterceptorKey key, params object[] args);
        object[] MakeArgs(NamedInterceptorValue value, object[] args);
        object[] MakeArgsWithHeader(NamedInterceptorValue value, object[] args);
    }
}