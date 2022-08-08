using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class DataFinderBase<TIdentity, TEntity> : OptionalDataFinder<TIdentity,TEntity>, IIdentityGenerater<TIdentity>, IDataFinder<TIdentity, TEntity>, IDataFinder, IDataFinderOptions<TIdentity, TEntity>
    {
        public async Task<TEntity> FindInCacheAsync(TIdentity identity)
        {
            var key = GetEntryKey(identity);
            var entity = await CoreFindInCacheAsync(key, identity);
            if (entity != null && CanRenewal(identity))
            {
                await RenewalAsync(identity);
            }
            return entity;
        }

        protected abstract Task<TEntity> CoreFindInCacheAsync(string key, TIdentity identity);

        public async Task<TEntity> FindInDbAsync(TIdentity identity, bool cache = true)
        {
            var entry = await OnFindInDbAsync(identity);
            if (entry != null && cache)
            {
                await SetInCacheAsync(identity, entry);
            }
            return entry;
        }

        protected abstract Task<TEntity> OnFindInDbAsync(TIdentity identity);

        public Task<bool> SetInCacheAsync(TIdentity identity, TEntity entity)
        {
            var key = GetEntryKey(identity);
            var cacheTime = GetCacheTime(identity);
            return SetInCacheAsync(key, identity, entity, cacheTime);
        }
        protected abstract Task<bool> SetInCacheAsync(string key, TIdentity identity, TEntity entity, TimeSpan? caheTime);

        public virtual TimeSpan? GetCacheTime(TIdentity identity)
        {
            return Options.GetCacheTime(identity);
        }

        public abstract Task<bool> DeleteAsync(TIdentity entity);
        public abstract Task<bool> ExistsAsync(TIdentity identity);

        public virtual bool CanRenewal(TIdentity identity)
        {
            return Options.CanRenewal(identity);
        }

        public abstract Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time);

        public string GetEntryKey(TIdentity identity)
        {
            return Options.GetEntryKey(identity);
        }

        public string GetHead()
        {
            return Options.GetHead();
        }

        public string GetPart(TIdentity identity)
        {
            return Options.GetPart(identity);
        }

        Task<bool> IDataFinder.DeleteAsync(object identity)
        {
            return DeleteAsync((TIdentity)identity);
        }

        Task<bool> IDataFinder.ExistsAsync(object identity)
        {
            return ExistsAsync((TIdentity)identity);
        }

        Task<bool> ICacheFinder.SetInCacheAsync(object identity, object entity)
        {
            return SetInCacheAsync((TIdentity)identity, (TEntity)entity);
        }

        async Task<object> ICacheFinder.FindInCacheAsync(object identity)
        {
            var res = await FindInCacheAsync((TIdentity)identity);
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

        public Task<bool> RenewalAsync(TIdentity identity)
        {
            return RenewalAsync(identity, GetCacheTime(identity));
        }

        Task<bool> IRenewalable.RenewalAsync(object identity)
        {
            return RenewalAsync((TIdentity)identity, GetCacheTime((TIdentity)identity));
        }
    }

}
