using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb
{
    public abstract class LitedbCacheBatchFinder<TIdentity, TEntry> : LitedbCacheBatchFinder<TIdentity, TEntry, TEntry>
       where TEntry : ILiteCacheEntity, new()
    {
        protected LitedbCacheBatchFinder(ILiteCollection<TEntry> collection) : base(collection)
        {
        }

        protected override Expression<Func<TEntry, TEntry>> GetSelect()
        {
            return x => x;
        }

        protected override TEntry ToCollectionEntity(TIdentity identity, TEntry entry)
        {
            return entry;
        }
    }
    public abstract class LitedbCacheBatchFinder<TIdentity, TEntry, TCollectionEntity> : BatchDataFinderBase<TIdentity, TEntry>
           where TCollectionEntity : TEntry, ILiteCacheEntity, new()
    {
        protected LitedbCacheBatchFinder(ILiteCollection<TCollectionEntity> collection)
        {
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public ILiteCollection<TCollectionEntity> Collection { get; }

        protected abstract Expression<Func<TCollectionEntity, bool>> GetWhere(IReadOnlyList<TIdentity> identity);
        protected abstract Expression<Func<TCollectionEntity, bool>> GetWhere(TIdentity identity);
        protected abstract Expression<Func<TCollectionEntity, TEntry>> GetSelect();
        protected abstract Expression<Func<TCollectionEntity, TIdentity>> GetIdentity();
        protected abstract TIdentity GetIdentity(TCollectionEntity entity);
        protected abstract TCollectionEntity ToCollectionEntity(TIdentity identity, TEntry entry);

        public override Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity)
        {
            var i = Collection.DeleteMany(GetWhere(identity));

            return Task.FromResult<long>(i);
        }

        public override Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity)
        {
            var res = Collection.Query()
                .Where(GetWhere(identity))
                .Select(GetIdentity())
                .ToList();
            var map = new Dictionary<TIdentity, bool>(identity.Count);
            for (int i = 0; i < identity.Count; i++)
            {
                var iden = identity[i];
                map[iden] = res.Contains(iden);
            }
            return Task.FromResult<IDictionary<TIdentity, bool>>(map);
        }

        public override async Task<IDictionary<TIdentity, TEntry>> FindInCahceAsync(IReadOnlyList<TIdentity> identity)
        {
            var coll = Collection;
            var datas = coll.Query()
                .Where(GetWhere(identity))
                .ToList();
            var now = DateTime.Now;
            var rm = new List<TIdentity>();
            var map = new Dictionary<TIdentity, TEntry>();
            for (int i = 0; i < datas.Count; i++)
            {
                var data = datas[i];
                if (data == null || data.ExpirationTime == null || data.ExpirationTime >= now)
                {
                    map[GetIdentity(data)] = data;
                }
            }
            if (rm.Count != 0)
            {
                await DeleteAsync(rm);
            }
            return map;
        }

        public override Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input)
        {
            var res = 0L;
            foreach (var item in input)
            {
                var t = GetExpirationTime(item.Value);
                var c = Collection.UpdateMany(x => new TCollectionEntity { ExpirationTime = t }, GetWhere(item.Key));
                if (c > 0)
                {
                    res++;
                }
            }
            return Task.FromResult(res);
        }
        protected DateTime? GetExpirationTime(TimeSpan? time)
        {
            return time == null ? (DateTime?)null : DateTime.Now.Add(time.Value);
        }

        public override Task<long> SetInCahceAsync(IDictionary<TIdentity, TEntry> pairs)
        {
            var ds = Collection.Query()
                .Where(GetWhere(pairs.Keys.ToList()))
                .ToList();
            var notIn = pairs.Keys.Except(ds.Select(x => GetIdentity(x)));
            var inserts = new List<TCollectionEntity>();
            var ok = 0L;
            foreach (var item in notIn)
            {
                var entity = pairs[item];
                var time = GetCacheTime(item);
                var exprTime = GetExpirationTime(time);
                var row = ToCollectionEntity(item, entity);
                row.ExpirationTime = exprTime;
                inserts.Add(row);
            }
            ok += Collection.InsertBulk(inserts);
            foreach (var item in ds)
            {
                var time = GetCacheTime(GetIdentity(item));
                var exprTime = GetExpirationTime(time);
                item.ExpirationTime = exprTime;
            }
            ok += Collection.Update(ds);
            return Task.FromResult(ok);
        }

    }
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
        where TCollectionEntity : TEntry, ILiteCacheEntity, new()
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

        protected override Task<bool> SetInCahceAsync(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
        {
            var newTime = GetExpirationTime(caheTime);
            var ent = Collection.Query().Where(GetWhere(identity)).FirstOrDefault();
            if (ent == null)
            {
                var row = ToCollectionEntity(identity, entity);
                Collection.Insert(row);
                row.ExpirationTime = newTime;
            }
            else
            {
                ent.ExpirationTime = newTime;
                Collection.Update(ent);
            }
            return Task.FromResult(true);
        }
    }
}
