﻿using System;
using System.Reflection;

namespace Ao.Cache.Proxy.Interceptors
{
    public readonly struct NamedInterceptorKey : IEquatable<NamedInterceptorKey>
    {
        public NamedInterceptorKey(Type targetType, MethodBase method)
        {
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            Method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public Type TargetType { get; }

        public MethodBase Method { get; }

        public override int GetHashCode()
        {
            var h1 = TargetType.GetHashCode();
            var h2 = Method.GetHashCode();
            return (h1 << 5) + h1 ^ h2;
        }
        public override bool Equals(object obj)
        {
            return Equals((NamedInterceptorKey)obj);
        }

        public bool Equals(NamedInterceptorKey other)
        {
            return other.TargetType == TargetType &&
                other.Method == Method;
        }
        public override string ToString()
        {
            return $"{{TargetType: {TargetType}, Method: {Method}}}";
        }

        public static bool operator ==(NamedInterceptorKey left, NamedInterceptorKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NamedInterceptorKey left, NamedInterceptorKey right)
        {
            return !(left == right);
        }
    }
}