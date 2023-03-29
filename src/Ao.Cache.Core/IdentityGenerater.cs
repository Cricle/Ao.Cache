#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace Ao.Cache
{
    public class IdentityGenerater<TIdentity, TEntity> : IIdentityGenerater<TIdentity>
    {
        public string Head { get; set; } = FriendlyNameHelper<TEntity>.FriendlyName;

        public virtual string GetPart(TIdentity identity)
        {
            return identity?.ToString();
        }

        public virtual string GetHead()
        {
            return Head;
        }

        public virtual string GetEntryKey(TIdentity identity)
        {
#if NET6_0_OR_GREATER
            var d = new DefaultInterpolatedStringHandler(3,0);
            d.AppendLiteral(GetHead());
            d.AppendLiteral(".");
            d.AppendLiteral(GetPart(identity));
            return d.ToStringAndClear();
#else
            return $"{GetHead()}.{GetPart(identity)}";
#endif
        }
    }

}
