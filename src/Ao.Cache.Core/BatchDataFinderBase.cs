using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class BatchDataFinderBase<TIdentity, TEntity> : OptionalDataFinder<TIdentity, TEntity>, IIdentityGenerater<TIdentity>, IBatchDataFinder<TIdentity, TEntity>, IBatchRenewalable<TIdentity>, IBatchDataFinder, IBatchRenewalable, IDataFinderOptions<TIdentity, TEntity>
    {
        public abstract Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity);
        public abstract Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity);

        public async Task<IDictionary<TIdentity, TEntity>> FindInCacheAsync(IReadOnlyList<TIdentity> identity)
        {
            var entity = await CoreFindInCacheAsync(identity);
            if (entity.Count!=0)
            {
                var renewals = entity.Keys.Where(x => CanRenewal(x)).ToList();
                if (renewals.Count!=0)
                {
                    await RenewalAsync(renewals);
                }
            }
            return entity;
        }

        public async Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IReadOnlyList<TIdentity> identity, bool cache)
        {
            var entry = await OnFindInDbAsync(identity);
            if (entry != null && cache)
            {
                await SetInCacheAsync(entry);
            }
            return entry;
        }

        protected abstract Task<IDictionary<TIdentity, TEntity>> CoreFindInCacheAsync(IReadOnlyList<TIdentity> identity);

        public abstract Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input);

        public abstract Task<long> SetInCacheAsync(IDictionary<TIdentity, TEntity> pairs);

        public virtual TimeSpan? GetCacheTime(TIdentity identity)
        {
            return Options.GetCacheTime(identity);
        }
        public virtual bool CanRenewal(TIdentity identity)
        {
            return Options.CanRenewal(identity);
        }

        protected abstract Task<IDictionary<TIdentity, TEntity>> OnFindInDbAsync(IReadOnlyList<TIdentity> identities);

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
        Task<long> IBatchDataFinder.DeleteAsync(IList identity)
        {
            return DeleteAsync(identity.Cast<TIdentity>().ToList());
        }

        async Task<IDictionary<object, bool>> IBatchDataFinder.ExistsAsync(IList identity)
        {
            var res = await ExistsAsync(identity.Cast<TIdentity>().ToList());

            return res.ToDictionary(x => (object)x, x => x.Value);
        }

        Task<long> IBatchCacheFinder.SetInCacheAsync(IDictionary pairs)
        {
            var m = new Dictionary<TIdentity, TEntity>(pairs.Count);
            var enu = pairs.GetEnumerator();
            while (enu.MoveNext())
            {
                m[(TIdentity)enu.Key] = (TEntity)enu.Value;
            }
            return SetInCacheAsync(m);
        }

        async Task<IDictionary> IBatchCacheFinder.FindInCacheAsync(IList identity)
        {
            var res = await FindInCacheAsync(identity.Cast<TIdentity>().ToList());
            if (res is IDictionary map)
            {
                return map;
            }
            return res.ToDictionary(x => x.Key, x => x.Value);
        }

        async Task<IDictionary> IBatchPhysicalFinder.FindInDbAsync(IList identity, bool cache)
        {
            var res = await FindInDbAsync(identity.Cast<TIdentity>().ToList(), cache);
            if (res is IDictionary map)
            {
                return map;
            }
            return res.ToDictionary(x => x.Key, x => x.Value);
        }

        Task<long> IBatchRenewalable.RenewalAsync(IDictionary<object, TimeSpan?> input)
        {
            var map = input.ToDictionary(x => (TIdentity)x.Key, x => x.Value);
            return RenewalAsync(map);
        }

        public Task<long> RenewalAsync(IReadOnlyList<TIdentity> input)
        {
            var map = new Dictionary<TIdentity, TimeSpan?>(input.Count);
            foreach (var item in input)
            {
                var time=GetCacheTime(item);
                map[item] = time;
            }
            return RenewalAsync(map);
        }

        Task<long> IBatchRenewalable.RenewalAsync(IReadOnlyList<object> input)
        {
            var map = new Dictionary<TIdentity, TimeSpan?>(input.Count);
            foreach (var item in input)
            {
                var time = GetCacheTime((TIdentity)item);
                map[(TIdentity)item] = time;
            }
            return RenewalAsync(map);
        }
    }

}
