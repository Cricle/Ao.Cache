using System;

namespace Ao.Cache.CastleProxy
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
            unchecked
            {
                var h = 31 * 17 + Objects.GetHashCode();
                h = 31 * h + Header.GetHashCode();
                return h * 31 + ObjectTransfer.GetHashCode();
            }
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
            return other.Objects == Objects &&
                other.ObjectTransfer == ObjectTransfer &&
                other.Header == Header;
        }
    }
}
