using System;
using System.Linq;

namespace Ao.Cache.Proxy
{
    public readonly struct UnwindObject : IEquatable<UnwindObject>
    {
        public UnwindObject(object header, object[] objects, IStringTransfer objectTransfer)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Objects = objects ?? throw new ArgumentNullException(nameof(objects));
            ObjectTransfer = objectTransfer ?? throw new ArgumentNullException(nameof(objectTransfer));
        }

        public readonly object Header;

        public readonly object[] Objects;

        public readonly IStringTransfer ObjectTransfer;

        public override string ToString()
        {
            return ObjectTransfer.Combine(Header, Objects);
        }
        public override int GetHashCode()
        {
            var hs = new HashCode();
            hs.Add(Header);
            for (int i = 0; i < Objects.Length; i++)
            {
                hs.Add(Objects[i]);
            }
            hs.Add(ObjectTransfer);
            return hs.ToHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is UnwindObject)
            {
                return Equals((UnwindObject)obj);
            }
            return false;
        }

        public bool Equals(UnwindObject other)
        {
            return other.Objects.SequenceEqual(Objects)&&
                other.ObjectTransfer == ObjectTransfer &&
                other.Header == Header;
        }

        public static bool operator ==(UnwindObject left, UnwindObject right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UnwindObject left, UnwindObject right)
        {
            return !(left == right);
        }
    }
}
