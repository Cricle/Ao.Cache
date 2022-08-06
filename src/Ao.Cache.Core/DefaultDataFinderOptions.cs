using System;

namespace Ao.Cache
{
    public class DefaultDataFinderOptions<TIdentity, TEntity> : IdentityGenerater<TIdentity, TEntity>, IDataFinderOptions<TIdentity, TEntity>
    {
        public static DefaultDataFinderOptions<TIdentity, TEntity> Default => new DefaultDataFinderOptions<TIdentity, TEntity>();

        public virtual bool CanRenewal(TIdentity identity, TEntity entity)
        {
            return true;
        }

        public virtual TimeSpan? GetCacheTime(TIdentity identity, TEntity entity)
        {
            return DataFinderConst.DefaultCacheTime;
        }
    }

}
