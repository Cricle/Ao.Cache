using Ao.Cache.InLitedb.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb
{
    public class LitedbBatchCacheFinder<TIdentity, TEntry> : BatchDataFinderBase<TIdentity, TEntry>
    {
        public LitedbBatchCacheFinder(ILiteDatabase database, ILiteCollection<LiteCacheEntity> collection, IEntityConvertor entityConvertor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public ILiteDatabase Database { get; }

        public ILiteCollection<LiteCacheEntity> Collection { get; }

        public IEntityConvertor EntityConvertor { get; }

        protected virtual Expression<Func<LiteCacheEntity, bool>> GetWhere(IReadOnlyList<TIdentity> identity)
        {
            var keys = new string[identity.Count];
            for (int i = 0; i < identity.Count; i++)
            {
                keys[i] = GetEntryKey(identity[i]);
            }
            return x => keys.Contains(x.Identity);
        }
        public override Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity)
        {
            return Task.FromResult(Delete(identity));
        }
        readonly struct IdentityString
        {
            public readonly Dictionary<TIdentity, string> Map;

            public readonly string[] Keys;

            public IdentityString(Dictionary<TIdentity, string> map, string[] keys)
            {
                Map = map;
                Keys = keys;
            }
        }
        private IdentityString GetIdentityString(IReadOnlyList<TIdentity> identity)
        {
            var keys = new Dictionary<TIdentity, string>(identity.Count);
            var scanKeys = new string[identity.Count];
            for (int i = 0; i < identity.Count; i++)
            {
                var item = identity[i];
                var k = GetEntryKey(item);
                keys[item] = k;
                scanKeys[i] = k;
            }
            return new IdentityString(keys, scanKeys);
        }
        public override Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity)
        {
            return Task.FromResult(Exists(identity));
        }

        public override Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input)
        {
            return Task.FromResult(Renewal(input));
        }

        public override Task<long> SetInCacheAsync(IDictionary<TIdentity, TEntry> pairs)
        {
            return Task.FromResult(SetInCache(pairs));
        }
        protected override Task<IDictionary<TIdentity, TEntry>> CoreFindInCacheAsync(IReadOnlyList<TIdentity> identity)
        {
            return Task.FromResult(CoreFindInCache(identity));
        }

        public override long Delete(IReadOnlyList<TIdentity> identity)
        {
            return Collection.DeleteMany(GetWhere(identity));
        }

        public override IDictionary<TIdentity, bool> Exists(IReadOnlyList<TIdentity> identity)
        {
            var keys = GetIdentityString(identity);
            var scanKeys = keys.Keys;
            var ix = Collection.Query()
                .Where(x => scanKeys.Contains(x.Identity))
                .Select(x => x.Identity)
                .ToEnumerable();
            var hash = new HashSet<string>(ix);
            var res = new Dictionary<TIdentity, bool>(identity.Count);
            foreach (var item in keys.Map)
            {
                res[item.Key] = hash.Contains(item.Value);
            }
            return res;
        }

        public override long SetInCache(IDictionary<TIdentity, TEntry> pairs)
        {
            var ok = Database.BeginTrans();
            if (!ok)
            {
                return 0L;
            }
            try
            {
                var keys = GetIdentityString(pairs.Keys.ToList());
                var scanKeys = keys.Keys;
                var ds = Collection.Query()
                    .Where(x => scanKeys.Contains(x.Identity))
                    .ToList();
                var inserts = new List<LiteCacheEntity>();
                var now = DateTime.Now;
                foreach (var x in pairs)
                {
                    var cacheTime = GetCacheTime(x.Key);
                    var identity = GetEntryKey(x.Key);
                    var oldEntity = ds.FirstOrDefault(w => w.Identity == identity);
                    var data = EntityConvertor.ToBytes(x.Value, typeof(TEntry));
                    var expireTime = cacheTime == null ? (DateTime?)null : now.Add(cacheTime.Value);
                    if (oldEntity != null)
                    {
                        oldEntity.ExpireTime = expireTime;
                        oldEntity.Data = data;
                        continue;
                    }
                    var entity = new LiteCacheEntity
                    {
                        CreateTime = now,
                        Data = data,
                        ExpireTime = expireTime,
                        Identity = identity
                    };
                    inserts.Add(entity);
                }
                var res = 0L;
                if (ds.Count != 0)
                {
                    res += Collection.Update(ds);
                }
                if (inserts.Count != 0)
                {
                    res += Collection.InsertBulk(inserts);
                }
                Database.Commit();
                return res;
            }
            catch
            {
                Database.Rollback();
                throw;
            }
        }

        protected override IDictionary<TIdentity, TEntry> CoreFindInCache(IReadOnlyList<TIdentity> identity)
        {
            var keys = GetIdentityString(identity);
            var scanKeys = keys.Keys;
            var ds = Collection.Query()
                .Where(x => scanKeys.Contains(x.Identity))
                .Select(x => new
                {
                    x.Identity,
                    x.Data
                }).ToList();
            var res = new Dictionary<TIdentity, TEntry>(ds.Count);
            for (int i = 0; i < ds.Count; i++)
            {
                var item = ds[i];
                var idxIndex = Array.IndexOf(scanKeys, item.Identity);
                var iden = identity[idxIndex];
                res[iden] = (TEntry)EntityConvertor.ToEntry(item.Data, typeof(TEntry));
            }
            return res;
        }

        public override long Renewal(IDictionary<TIdentity, TimeSpan?> input)
        {

            var keys = GetIdentityString(input.Keys.ToList());
            var scanKeys = keys.Keys;
            var ds = Collection.Query()
                .Where(x => scanKeys.Contains(x.Identity))
                .ToList();
            var now = DateTime.Now;
            foreach (var item in ds)
            {
                var v = keys.Map.FirstOrDefault(x => x.Value == item.Identity);
                if (v.Value != null && input.TryGetValue(v.Key, out var cacheTime))
                {
                    item.ExpireTime = cacheTime == null ? (DateTime?)null : now.Add(cacheTime.Value);
                }
            }
            var res = Collection.Update(ds);
            return res;
        }
    }
}
