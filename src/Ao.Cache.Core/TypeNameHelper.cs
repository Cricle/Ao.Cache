using System;
using System.Collections.Generic;

namespace Ao.Cache
{
    public static class TypeNameHelper
    {
        public static string GetFriendlyName(Type type)
        {
            if (type.IsGenericType)
            {
                var gens = type.GetGenericArguments();
                var names = new string[gens.Length];
                for (int i = 0; i < gens.Length; i++)
                {
                    var gen = gens[i];
                    names[i] = GetFriendlyName(gen);
                }
                var actualName = type.Name.Split('`')[0];
                return string.Concat(actualName, "<", string.Join(",", names), ">");
            }
            return type.Name;
        }
    }
}
