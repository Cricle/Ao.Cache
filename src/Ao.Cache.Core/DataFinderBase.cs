using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class DataFinderBase<TIdentity, TEntity> : IdentityGenerater<TIdentity, TEntity>, IDataFinder<TIdentity, TEntity>,IRenewalable<TIdentity>
    {
        protected DataFinderBase()
        {
        }

        public Task<TEntity> FindInCahceAsync(TIdentity identity)
        {
            var key = GetEntryKey(identity);
            return CoreFindInCacheAsync(key, identity);
        }

        protected abstract Task<TEntity> CoreFindInCacheAsync(string key, TIdentity identity);

        public async Task<TEntity> FindInDbAsync(TIdentity identity, bool cache = true)
        {
            var entry = await OnFindInDbAsync(identity);
            if (entry != null && cache)
            {
                await SetInCahceAsync(identity, entry);
            }
            return entry;
        }

        protected abstract Task<TEntity> OnFindInDbAsync(TIdentity identity);

        public Task<bool> SetInCahceAsync(TIdentity identity, TEntity entity)
        {
            var key = GetEntryKey(identity);
            var cacheTime = GetCacheTime(identity, entity);
            return SetInCahceAsync(key, identity, entity, cacheTime);
        }
        protected abstract Task<bool> SetInCahceAsync(string key, TIdentity identity, TEntity entity, TimeSpan? caheTime);
        
        protected virtual TimeSpan? GetCacheTime(TIdentity identity, TEntity entity)
        {
            return DataFinderConst.DefaultCacheTime;
        }

        public abstract Task<bool> DeleteAsync(TIdentity entity);
        public abstract Task<bool> ExistsAsync(TIdentity identity);

        protected virtual bool CanRenewal(TIdentity identity, TEntity entity)
        {
            return true;
        }

        public abstract Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time);
    }

}
