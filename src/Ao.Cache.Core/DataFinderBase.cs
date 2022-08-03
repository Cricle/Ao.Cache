using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class DataFinderBase<TIdentity, TEntity> : IdentityGenerater<TIdentity, TEntity>, IDataFinder<TIdentity, TEntity>,IRenewalable<TIdentity>, IDataFinder,IRenewalable
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

        Task<bool> IDataFinder.DeleteAsync(object identity)
        {
            return DeleteAsync((TIdentity)identity);
        }

        Task<bool> IDataFinder.ExistsAsync(object identity)
        {
            return ExistsAsync((TIdentity)identity);
        }

        Task<bool> ICacheFinder.SetInCahceAsync(object identity, object entity)
        {
            return SetInCahceAsync((TIdentity)identity,(TEntity)entity);
        }

        async Task<object> ICacheFinder.FindInCahceAsync(object identity)
        {
            var res=await FindInCahceAsync((TIdentity)identity);
            return res;
        }

        async Task<object> IPhysicalFinder.FindInDbAsync(object identity, bool cache)
        {
            var res = await FindInDbAsync((TIdentity)identity);
            return res;
        }

        Task<bool> IRenewalable.RenewalAsync(object identity, TimeSpan? time)
        {
            return RenewalAsync((TIdentity)identity, time);
        }
    }

}
