using System;
using System.Collections.Generic;

namespace Ao.Cache
{
    public static class TypeNameHelper
    {
        private static readonly object SycRoot = new object();
        private static readonly Dictionary<Type, string> TypeNameCaches = new Dictionary<Type, string>();

        public static string GetFriendlyFullName(Type type)
        {
            if (!TypeNameCaches.TryGetValue(type, out var name))
            {
                lock (SycRoot)
                {
                    if (!TypeNameCaches.TryGetValue(type, out name))
                    {
                        name = string.Concat(type.FullName.Split('`')[0], GetFriendlyName(type));
                        TypeNameCaches[type] = name;
                    }
                }
            }
            return name;
        }
        private static string GetFriendlyName(Type type)
        {
            var genType = type.GenericTypeArguments;
            if (genType != null && genType.Length != 0)
            {
                var names = new string[genType.Length];
                for (int i = 0; i < genType.Length; i++)
                {
                    var gen = genType[i];
                    names[i] = GetFriendlyName(gen);
                }
                var actualName = type.Name.Split('`')[0];
                return string.Concat(actualName, "<", string.Join(",", names), ">");
            }
            return type.Name;
        }
    }
}
