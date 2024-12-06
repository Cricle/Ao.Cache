using System.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Ao.Cache
{
    internal readonly struct DeclareInfo : IEquatable<DeclareInfo>
    {
        public readonly Type Type;

        public readonly MethodInfo Method;

        public readonly int Hash;

        public DeclareInfo(Type type, MethodInfo method)
        {
            Type = type;
            Method = method;
#if NETSTANDARD2_0
            Hash= Type?.GetHashCode() ?? 0 ^ Method?.GetHashCode() ?? 0;
#else
            Hash = HashCode.Combine(Type, Method);
#endif

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return Hash;
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
            return other.Hash == Hash && other.Type == Type && other.Method == Method;
        }
    }

}
