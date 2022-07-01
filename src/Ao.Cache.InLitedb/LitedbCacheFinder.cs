using LiteDB;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb
{
    public abstract class LitedbCacheFinder<TIdentity, TEntry> : LitedbCacheFinder<TIdentity, TEntry, TEntry>
          where TEntry : ILiteCacheEntity, new()
    {
        protected LitedbCacheFinder(ILiteCollection<TEntry> collection) 
            : base(collection)
        {
        }

        protected override Expression<Func<TEntry, TEntry>> GetSelect(TIdentity identity)
        {
            return x => x;
        }

        protected override TEntry ToCollectionEntity(TIdentity identity, TEntry entry)
        {
            return entry;
        }
    }
    public abstract partial class LitedbCacheFinder<TIdentity, TEntry, TCollectionEntity> : DataFinderBase<TIdentity, TEntry>
        where TCollectionEntity : TEntry,ILiteCacheEntity, new()
    {
        protected LitedbCacheFinder(ILiteCollection<TCollectionEntity> collection)
        {
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public ILiteCollection<TCollectionEntity> Collection { get; }

        protected abstract Expression<Func<TCollectionEntity, bool>> GetWhere(TIdentity identity);
        protected abstract Expression<Func<TCollectionEntity, TEntry>> GetSelect(TIdentity identity);
        protected abstract TCollectionEntity ToCollectionEntity(TIdentity identity, TEntry entry);

        public override Task<bool> DeleteAsync(TIdentity entity)
        {
            var i = Collection.DeleteMany(GetWhere(entity));

            return Task.FromResult(i != 0);
        }

        public override Task<bool> ExistsAsync(TIdentity identity)
        {
            var res = Collection.Exists(GetWhere(identity));

            return Task.FromResult(res);
        }
        protected DateTime? GetExpirationTime(TimeSpan? time)
        {
            return time == null ? (DateTime?)null : DateTime.Now.Add(time.Value);
        }
        public override Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            var newTime = GetExpirationTime(time);

            var r = Collection
                .UpdateMany(x => new TCollectionEntity { ExpirationTime = newTime },
                    GetWhere(identity));
            return Task.FromResult(r != 0);
        }

        protected override async Task<TEntry> CoreFindInCacheAsync(string key, TIdentity identity)
        {
            var coll = Collection;
            var data = coll.Query()
                .Where(GetWhere(identity))
                .Limit(1)
                .FirstOrDefault();
            var now = DateTime.Now;
            if (data != null && data.ExpirationTime != null && data.ExpirationTime < now)
            {
                await DeleteAsync(identity);
                return default;
            }
            return data;
        }

        protected override Task<bool> WriteCacheAsync(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
        {
            var row = ToCollectionEntity(identity, entity);
            var newTime = GetExpirationTime(caheTime);
            row.ExpirationTime = newTime;
            Collection
                 .Insert(row);
            return Task.FromResult(true);
        }
    }
}
