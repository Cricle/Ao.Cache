using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class DataFinderBase<TIdentity, TEntity> : OptionalDataFinder<TIdentity, TEntity>, IIdentityGenerater<TIdentity>, IDataFinder<TIdentity, TEntity>, IDataFinderOptions<TIdentity, TEntity>
    {
        public async Task<TEntity> FindInCacheAsync(TIdentity identity)
        {
            var key = GetEntryKey(identity);
            var entity = await CoreFindInCacheAsync(key, identity);
            if (CanRenewal(identity)&&entity != null)
            {
                await RenewalAsync(identity);
            }
            return entity;
        }

        protected abstract Task<TEntity> CoreFindInCacheAsync(string key, TIdentity identity);

        public async Task<TEntity> FindInDbAsync(IDataAccesstor<TIdentity, TEntity> dataAccesstor, TIdentity identity, bool cache = true)
        {
            var entry = await dataAccesstor.FindAsync(identity);
            if (entry != null && cache)
            {
                await SetInCacheAsync(identity, entry);
            }
            return entry;
        }

        public Task<bool> SetInCacheAsync(TIdentity identity, TEntity entity)
        {
            var key = GetEntryKey(identity);
            var cacheTime = GetCacheTime(identity);
            return SetInCacheAsync(key, identity, entity, cacheTime);
        }
        protected abstract Task<bool> SetInCacheAsync(string key, TIdentity identity, TEntity entity, TimeSpan? caheTime);


        public abstract Task<bool> DeleteAsync(TIdentity entity);
        public abstract Task<bool> ExistsAsync(TIdentity identity);

        public abstract Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time);

        public Task<bool> RenewalAsync(TIdentity identity)
        {
            return RenewalAsync(identity, GetCacheTime(identity));
        }

    }

}
