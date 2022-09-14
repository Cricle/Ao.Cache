using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ao.Cache.Proxy
{
    public static class CastleTypeHelper
    {
        private static readonly object locker = new object();
        private static readonly Dictionary<Type, FieldInfo> fieldInfos = new Dictionary<Type, FieldInfo>();

        public static FieldInfo GetActualFieldInfo(Type type)
        {
            if (!fieldInfos.TryGetValue(type, out var t))
            {
                lock (locker)
                {
                    if (!fieldInfos.TryGetValue(type, out t))
                    {
                        t = type.GetField("__target", BindingFlags.Instance | BindingFlags.NonPublic);
                        fieldInfos[type] = t;
                    }
                }
            }
            return t;
        }
    }
}