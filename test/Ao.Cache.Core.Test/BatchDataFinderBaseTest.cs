using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    internal class MemoryBatchDataFinderBase<TIdentity, TEntity> : BatchDataFinderBase<TIdentity, TEntity>
    {
        public readonly ConcurrentDictionary<TIdentity, TEntity> Datas = new ConcurrentDictionary<TIdentity, TEntity>();

        public override Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity)
        {
            return Task.FromResult(Delete(identity));
        }

        public override Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity)
        {
            return Task.FromResult(Exists(identity));
        }

        public override Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input)
        {
            return Task.FromResult(0L);
        }

        public override Task<long> SetInCacheAsync(IDictionary<TIdentity, TEntity> pairs)
        {
            return Task.FromResult(SetInCache(pairs));
        }

        protected override Task<IDictionary<TIdentity, TEntity>> CoreFindInCacheAsync(IReadOnlyList<TIdentity> identity)
        {
            return Task.FromResult(CoreFindInCache(identity));
        }
        public override Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IBatchDataAccesstor<TIdentity, TEntity> batchDataAccesstor, IReadOnlyList<TIdentity> identity, bool cache)
        {
            return Task.FromResult<IDictionary<TIdentity, TEntity>>(new Dictionary<TIdentity, TEntity>());
        }

        public override long Delete(IReadOnlyList<TIdentity> identity)
        {
            var r = 0L;
            foreach (var item in identity)
            {
                if (Datas.TryRemove(item, out _))
                {
                    r++;
                }
            }
            return r;
        }

        public override IDictionary<TIdentity, bool> Exists(IReadOnlyList<TIdentity> identity)
        {
            var map = new Dictionary<TIdentity, bool>();
            foreach (var item in identity)
            {
                map[item] = Datas.ContainsKey(item);
            }
            return map;
        }

        public override long SetInCache(IDictionary<TIdentity, TEntity> pairs)
        {
            foreach (var item in pairs)
            {
                Datas[item.Key] = item.Value;
            }
            return pairs.Count;
        }

        protected override IDictionary<TIdentity, TEntity> CoreFindInCache(IReadOnlyList<TIdentity> identity)
        {
            var m = new Dictionary<TIdentity, TEntity>();
            foreach (var item in identity)
            {
                if (Datas.TryGetValue(item, out var val))
                {
                    m[item] = val;
                }
            }
            return m;
        }

        public override long Renewal(IDictionary<TIdentity, TimeSpan?> input)
        {
            return 0L;
        }
    }
    [TestClass]
    public class BatchDataFinderBaseTest
    {
    }
}
