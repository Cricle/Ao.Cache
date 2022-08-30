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
            var r = 0L;
            foreach (var item in identity)
            {
                if (Datas.TryRemove(item, out _))
                {
                    r++;
                }
            }
            return Task.FromResult(r);
        }

        public override Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity)
        {
            var map = new Dictionary<TIdentity, bool>();
            foreach (var item in identity)
            {
                map[item] = Datas.ContainsKey(item);
            }
            return Task.FromResult<IDictionary<TIdentity, bool>>(map);
        }

        public override Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input)
        {
            return Task.FromResult(0L);
        }

        public override Task<long> SetInCacheAsync(IDictionary<TIdentity, TEntity> pairs)
        {
            foreach (var item in pairs)
            {
                Datas[item.Key] = item.Value;
            }
            return Task.FromResult<long>(pairs.Count);
        }

        protected override Task<IDictionary<TIdentity, TEntity>> CoreFindInCacheAsync(IReadOnlyList<TIdentity> identity)
        {
            var m = new Dictionary<TIdentity, TEntity>();
            foreach (var item in identity)
            {
                if (Datas.TryGetValue(item, out var val))
                {
                    m[item] = val;
                }
            }
            return Task.FromResult<IDictionary<TIdentity, TEntity>>(m);
        }

        protected override Task<IDictionary<TIdentity, TEntity>> OnFindInDbAsync(IReadOnlyList<TIdentity> identities)
        {
            return Task.FromResult<IDictionary<TIdentity, TEntity>>(new Dictionary<TIdentity, TEntity>());
        }
    }
    [TestClass]
    public class BatchDataFinderBaseTest
    {
    }
}
