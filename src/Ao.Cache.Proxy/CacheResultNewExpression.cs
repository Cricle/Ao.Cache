using Ao.Cache.Proxy.Model;
using System;
using System.Linq.Expressions;

namespace Ao.Cache.Proxy
{
    public static class CacheResultNewExpression<T>
    {
        public static readonly Type Type = typeof(T);

        public static readonly Func<T> Creator;

        public static readonly Type GenericType;

        public static readonly bool IsAutoResult;

        static CacheResultNewExpression()
        {
            IsAutoResult = Type.IsGenericType &&
                Type.GetGenericTypeDefinition() == typeof(AutoCacheResult<>);
            if (IsAutoResult)
            {
                GenericType = Type.GenericTypeArguments[0];
            }
            Creator = Expression.Lambda<Func<T>>(Expression.New(Type)).Compile();
        }
    }
}
