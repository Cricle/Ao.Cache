namespace Ao.Cache
{
    public class IdentityGenerater<TIdentity, TEntity> : IIdentityGenerater<TIdentity>
    {
        public virtual string GetPart(TIdentity identity)
        {
            return identity?.ToString();
        }
        public virtual string GetHead()
        {
            return FriendlyHelper<TEntity>.FriendlyName;
        }
        public virtual string GetEntryKey(TIdentity identity)
        {
            return string.Concat(GetHead(), ".", GetPart(identity));
        }
    }

}
