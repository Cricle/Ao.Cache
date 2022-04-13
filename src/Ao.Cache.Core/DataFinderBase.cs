using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class DataFinderBase<TIdentity, TEntity> : IDataFinder<TIdentity, TEntity>
    {
        protected static readonly Type EntityType = typeof(TEntity);

        public static readonly TimeSpan DefaultCacheTime = TimeSpan.FromSeconds(3);
        
        public static readonly string EntryFriendlyName = TypeNameHelper.GetFriendlyFullName(typeof(TEntity));

        protected DataFinderBase()
        {
        }

        public Task<TEntity> FindInCahceAsync(TIdentity identity)
        {
            var key = GetEntryKey(identity);
            return CoreFindInCacheAsync(key, identity);
        }

        protected abstract Task<TEntity> CoreFindInCacheAsync(string key, TIdentity identity);

        public virtual string GetPart(TIdentity identity)
        {
            return identity?.ToString();
        }
        public virtual string GetHead()
        {
            return EntryFriendlyName;
        }
        public string GetEntryKey(TIdentity identity)
        {
            return string.Concat(GetHead(), ".", GetPart(identity));
        }
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
            return WriteCacheAsync(key, identity, entity, cacheTime);
        }
        protected abstract Task<bool> WriteCacheAsync(string key, TIdentity identity, TEntity entity, TimeSpan? caheTime);
        
        protected virtual TimeSpan? GetCacheTime(TIdentity identity, TEntity entity)
        {
            return DefaultCacheTime;
        }

        public abstract Task<bool> DeleteAsync(TIdentity entity);
        public abstract Task<bool> ExistsAsync(TIdentity identity);
    }

}
