using System;
using System.Collections.Generic;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public class NamedInterceptorValue : IEquatable<NamedInterceptorValue>
    {
        public NamedInterceptorValue(IReadOnlyList<int> argIndexs, string header)
        {
            ArgIndexs = argIndexs;
            Header = header ?? throw new ArgumentNullException(nameof(header));
        }

        public IReadOnlyList<int> ArgIndexs { get; }

        public string Header { get; }

        public override int GetHashCode()
        {
            var h1 = ArgIndexs.GetHashCode();
            var h2 = Header.GetHashCode();
            return ((h1 << 5) + h1) ^ h2;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as NamedInterceptorValue);
        }

        public bool Equals(NamedInterceptorValue other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ArgIndexs == ArgIndexs &&
                other.Header == Header;
        }
        public override string ToString()
        {
            return $"{{ArgIndexs: {ArgIndexs}, Header: {Header}}}";
        }
    }
}
