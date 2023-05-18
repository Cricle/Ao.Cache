namespace Ao.Cache
{
    public class WithHeaderIdentityGenerater<TIdentity,TEntity> : IdentityGenerater<TIdentity, TEntity>, IWithHeaderIdentityGenerater<TIdentity>
    {
        private bool ignoreHead;

        public bool IgnoreHead
        {
            get => ignoreHead;
            set=> ignoreHead = value;
        }

        public override string GetEntryKey(TIdentity identity)
        {
            if (ignoreHead)
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
