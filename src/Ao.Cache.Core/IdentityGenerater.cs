namespace Ao.Cache
{
    public class IdentityGenerater<TIdentity, TEntity> : IWithHeaderIdentityGenerater<TIdentity>
    {
        public bool IgnoreHead { get; set; }

        public virtual string GetPart(TIdentity identity)
        {
            return identity?.ToString();
        }

        public virtual string GetHead()
        {
            return FriendlyNameHelper<TEntity>.FriendlyName;
        }

        public virtual string GetEntryKey(TIdentity identity)
        {
            if (IgnoreHead)
            {
                return GetPart(identity);
            }
            else
            {
                return string.Concat(GetHead(), ".", GetPart(identity));
            }
        }
    }

}
