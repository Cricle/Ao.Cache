using System;
using System.Collections.Generic;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public class NamedInterceptorValue : IEquatable<NamedInterceptorValue>
    {
        public NamedInterceptorValue(IReadOnlyList<int> argIndexs, IStringTransfer stringTransfer, string header)
        {
            ArgIndexs = argIndexs;
            StringTransfer = stringTransfer ?? throw new ArgumentNullException(nameof(stringTransfer));
            Header = header ?? throw new ArgumentNullException(nameof(header));
        }

        public IReadOnlyList<int> ArgIndexs { get; }

        public IStringTransfer StringTransfer { get; }

        public string Header { get; }

        public override int GetHashCode()
        {
            var h1 = ArgIndexs?.GetHashCode() ?? 0;
            var h2 = Header.GetHashCode();
            var h3 = StringTransfer.GetHashCode();
            unchecked
            {
                var h = 31 * 17 + h1;
                h = h * 31 + h2;
                h = h * 31 + h3;
                return h;
            }
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
                other.Header == Header &&
                other.StringTransfer == StringTransfer;
        }
        public override string ToString()
        {
            return $"{{ArgIndexs: {ArgIndexs}, Header: {Header}, StringTransfer: {StringTransfer}}}";
        }
    }
}
