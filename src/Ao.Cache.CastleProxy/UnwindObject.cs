using System;

namespace Ao.Cache.CastleProxy
{
    public readonly struct UnwindObject : IEquatable<UnwindObject>
    {
        public UnwindObject(object[] objects, IStringTransfer objectTransfer)
        {
            Objects = objects ?? throw new ArgumentNullException(nameof(objects));
            ObjectTransfer = objectTransfer ?? throw new ArgumentNullException(nameof(objectTransfer));
        }

        public readonly object[] Objects;

        public readonly IStringTransfer ObjectTransfer;

        public override string ToString()
        {
            return ObjectTransfer.Combine(Objects);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var h = 31 * 17 + Objects.GetHashCode();
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
                other.ObjectTransfer == ObjectTransfer;
        }
    }
}
