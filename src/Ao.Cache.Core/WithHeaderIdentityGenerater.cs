namespace Ao.Cache
{
    public class WithHeaderIdentityGenerater<TIdentity, TEntity> : IdentityGenerater<TIdentity, TEntity>, IWithHeaderIdentityGenerater<TIdentity>
    {
        public bool IgnoreHead { get; set; }

        public override string GetEntryKey(TIdentity identity)
        {
            if (IgnoreHead)
            {
                return GetPart(identity);
            }
            else
            {
                return base.GetEntryKey(identity);
            }
        }
    }

}
