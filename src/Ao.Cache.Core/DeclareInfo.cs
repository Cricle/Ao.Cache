using System.Reflection;
using System;

namespace Ao.Cache
{
    internal readonly struct DeclareInfo : IEquatable<DeclareInfo>
    {
        public readonly Type Type;

        public readonly MethodInfo Method;

        public DeclareInfo(Type type, MethodInfo method)
        {
            Type = type;
            Method = method;
        }

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            return Type?.GetHashCode() ?? 0 ^ Method?.GetHashCode() ?? 0;
#else
            return HashCode.Combine(Type, Method);
#endif
        }
        public override bool Equals(object obj)
        {
            if (obj is DeclareInfo info)
            {
                return Equals(info);
            }
            return false;
        }

        public bool Equals(DeclareInfo other)
        {
            return other.Type == Type && other.Method == Method;
        }
    }

}
