using Ao.Cache.InLitedb.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb
{
    public abstract class LitedbCacheFinder<TIdentity, TEntry> : DataFinderBase<TIdentity, TEntry>
    {
        protected LitedbCacheFinder(ILiteDatabase database, ILiteCollection<LiteCacheEntity> collection, IEntityConvertor entityConvertor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public ILiteDatabase Database { get; }

        public ILiteCollection<LiteCacheEntity> Collection { get; }

        public IEntityConvertor EntityConvertor { get; }

        protected virtual Expression<Func<LiteCacheEntity, bool>> GetWhere(TIdentity identity)
        {
            var key = GetEntryKey(identity);
            return x => x.Identity == key;
        }
        protected virtual LiteCacheEntity ToCollectionEntity(TIdentity identity, TEntry entry)
        {
            var now = DateTime.Now;
            var cacheTime = GetCacheTime(identity);
            return new LiteCacheEntity
            {
                CreateTime = DateTime.Now,
                Data = EntityConvertor.ToBytes(entry, typeof(TEntry)),
                Identity = GetEntryKey(identity),
                ExpireTime = cacheTime == null ? (DateTime?)null : now.Add(cacheTime.Value)
            };
        }

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
            var key = GetEntryKey(identity);
            var d = Collection.Query()
                .Where(x => x.Identity == key)
                .OrderByDescending(x => x.ExpireTime)
                .FirstOrDefault();
            if (d == null)
            {
                return Task.FromResult(false);
            }
            d.ExpireTime = newTime;
            var ok = Collection.Update(d);
            return Task.FromResult(ok);
        }

        protected override Task<TEntry> CoreFindInCacheAsync(string key, TIdentity identity)
        {
            var coll = Collection;
            var data = coll.Query()
                .Where(GetWhere(identity))
                .ToList();
            if (data.Count == 0)
            {
                return Task.FromResult<TEntry>(default);
            }
            var now = DateTime.Now;
            var rms = new List<ObjectId>();
            byte[] val = null;
            DateTime? dt = default;
            bool ok = false;
            foreach (var item in data)
            {
                if (data != null && item.ExpireTime != null && item.ExpireTime < now)
                {
                    rms.Add(item.Id);
                }
                else
                {
                    if (!ok)
                    {
                        ok = true;
                        val = item.Data;
                        dt = item.ExpireTime;
                    }
                    else
                    {
                        if (dt != null)
                        {
                            if (item.ExpireTime == null)
                            {
                                val = item.Data;
                                dt = item.ExpireTime;
                            }
                            else if (item.ExpireTime > dt)
                            {
                                val = item.Data;
                                dt = item.ExpireTime;
                            }
                        }
                    }
                }
            }
            if (rms.Count != 0)
            {
                Collection.DeleteMany(x => rms.Contains(x.Id));
            }
            if (val != null)
            {
                return Task.FromResult((TEntry)EntityConvertor.ToEntry(val, typeof(TEntry)));
            }
            return Task.FromResult<TEntry>(default);
        }

        protected override Task<bool> SetInCacheAsync(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
        {
            var newTime = GetExpirationTime(caheTime);
            var row = ToCollectionEntity(identity, entity);
            var ent = Collection.Query().Where(GetWhere(identity)).OrderByDescending(x => x.ExpireTime).FirstOrDefault();
            if (ent == null)
            {
                Collection.Insert(row);
                row.ExpireTime = newTime;
            }
            else
            {
                ent.ExpireTime = newTime;
                Collection.Update(ent);
            }
            Database.Commit();
            return Task.FromResult(true);
        }
    }
}
