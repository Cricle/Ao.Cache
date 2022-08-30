using System;

namespace Ao.Cache
{
    public class DefaultDataFinderOptions<TIdentity, TEntity> : WithHeaderIdentityGenerater<TIdentity, TEntity>, IDataFinderOptions<TIdentity, TEntity>
    {
        public static DefaultDataFinderOptions<TIdentity, TEntity> Default => new DefaultDataFinderOptions<TIdentity, TEntity>();

        public bool IsCanRenewal { get; set; } = true;

        public TimeSpan? CacheTime { get; set; } = DataFinderConst.DefaultCacheTime;

        public virtual bool CanRenewal(TIdentity identity)
        {
            return IsCanRenewal;
        }

        public TimeSpan? GetCacheTime(TIdentity identity)
        {
            return CacheTime;
        }
    }

}
