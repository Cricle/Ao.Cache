using Ao.Cache.Proxy.Model;
using System;
using System.Collections.Generic;

namespace Ao.Cache.Proxy
{
    public static class ActionTypeHelper
    {
        private static readonly object actionTypeLocker = new object();

        private static readonly Dictionary<Type, ActualTypeInfos> actualTypes = new Dictionary<Type, ActualTypeInfos>();
        public static ActualTypeInfos GetActionType(Type type)
        {
            if (!actualTypes.TryGetValue(type, out var t))
            {
                lock (actionTypeLocker)
                {
                    if (!actualTypes.TryGetValue(type, out t))
                    {
                        var typeEquals = true;
                        Type actualType = type;
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(AutoCacheResult<>))
                        {
                            actualType = type.GenericTypeArguments[0];
                            typeEquals = false;
                        }
                        var finderType = typeof(IDataFinder<,>).MakeGenericType(typeof(UnwindObject), actualType);
                        t = new ActualTypeInfos(actualType, finderType, typeEquals);
                        actualTypes[type] = t;
                    }
                }
            }
            return t;
        }

    }
}
