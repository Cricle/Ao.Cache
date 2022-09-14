using System;

namespace Ao.Cache.Proxy.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class StringTransferAttribute : Attribute
    {
        private static readonly string IStringTransferFullName = typeof(IStringTransfer).FullName;
        public StringTransferAttribute(Type stringTransferType)
        {
            StringTransferType = stringTransferType ?? throw new ArgumentNullException(nameof(stringTransferType));
            if (stringTransferType.GetInterface(IStringTransferFullName) == null)
            {
                throw new ArgumentException($"{stringTransferType} is not implement {IStringTransferFullName}");
            }
        }

        public Type StringTransferType { get; }
    }
}
