using Ao.Cache.InLitedb.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb
{
    public class LitedbCacheFinder<TIdentity, TEntry> : DataFinderBase<TIdentity, TEntry>
    {
        public LitedbCacheFinder(ILiteDatabase database, ILiteCollection<LiteCacheEntity> collection, IEntityConvertor entityConvertor)
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
            return Task.FromResult(Delete(entity));
        }

        public override Task<bool> ExistsAsync(TIdentity identity)
        {
            return Task.FromResult(Exists(identity));
        }
        protected DateTime? GetExpirationTime(TimeSpan? time)
        {
            return time == null ? (DateTime?)null : DateTime.Now.Add(time.Value);
        }
        public override Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            return Task.FromResult(Renewal(identity,time));
        }

        protected override Task<TEntry> CoreFindInCacheAsync(string key, TIdentity identity)
        {
            return Task.FromResult(CoreFindInCache(key,identity));
        }

        protected override Task<bool> SetInCacheAsync(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
        {            
            return Task.FromResult(SetInCache(key,identity,entity,caheTime));
        }

        public override bool Delete(TIdentity identity)
        {
           return Collection.DeleteMany(GetWhere(identity))!=0;
        }

        public override bool Exists(TIdentity identity)
        {
            return Collection.Exists(GetWhere(identity));
        }

        protected override bool SetInCache(string key, TIdentity identity, TEntry entity, TimeSpan? caheTime)
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
            return true;
        }

        protected override TEntry CoreFindInCache(string key, TIdentity identity)
        {
            var coll = Collection;
            var data = coll.Query()
                .Where(GetWhere(identity))
                .ToList();
            if (data.Count == 0)
            {
                return default;
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
                return (TEntry)EntityConvertor.ToEntry(val, typeof(TEntry));
            }
            return default;
        }

        public override bool Renewal(TIdentity identity, TimeSpan? time)
        {
            var newTime = GetExpirationTime(time);
            var key = GetEntryKey(identity);
            var d = Collection.Query()
                .Where(x => x.Identity == key)
                .OrderByDescending(x => x.ExpireTime)
                .FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            d.ExpireTime = newTime;
            var ok = Collection.Update(d);
            return ok;
        }
    }
}
