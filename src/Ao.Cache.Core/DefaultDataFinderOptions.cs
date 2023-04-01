using System;

namespace Ao.Cache
{
    public class DefaultDataFinderOptions<TIdentity, TEntity> : WithHeaderIdentityGenerater<TIdentity, TEntity>, IDataFinderOptions<TIdentity, TEntity>
    {
        internal bool isCanRenewal = false;
        internal TimeSpan? cacheTime= DataFinderConst.DefaultCacheTime;

        public bool IsCanRenewal
        {
            get => isCanRenewal;
            set=>isCanRenewal = value;
        }

        public TimeSpan? CacheTime
        {
            get => cacheTime;
            set => cacheTime = value;
        }

        public virtual bool CanRenewal(TIdentity identity)
        {
            return isCanRenewal;
        }

        public TimeSpan? GetCacheTime(TIdentity identity)
        {
            return cacheTime;
        }
    }

}
