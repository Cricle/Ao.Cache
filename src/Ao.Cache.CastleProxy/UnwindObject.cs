using System;

namespace Ao.Cache.CastleProxy
{
    public class UnwindObject : IUnwindObject
    {
        public UnwindObject(object[] objects, IStringTransfer objectTransfer)
        {
            Objects = objects ?? throw new ArgumentNullException(nameof(objects));
            ObjectTransfer = objectTransfer ?? throw new ArgumentNullException(nameof(objectTransfer));
        }

        public object[] Objects { get; }

        public IStringTransfer ObjectTransfer { get; }

        public override string ToString()
        {
            return ObjectTransfer.Combine(Objects);
        }
    }
}
