using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class BatchDataFinderBase<TIdentity, TEntity> : IdentityGenerater<TIdentity,TEntity>, IBatchDataFinder<TIdentity, TEntity>, IBatchRenewalable<TIdentity>, IBatchDataFinder, IBatchRenewalable
    {
        public abstract Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity);
        public abstract Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity);

        public abstract Task<IDictionary<TIdentity, TEntity>> FindInCahceAsync(IReadOnlyList<TIdentity> identity);

        public async Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IReadOnlyList<TIdentity> identity, bool cache)
        {
            var entry = await OnFindInDbAsync(identity);
            if (entry != null && cache)
            {
                await SetInCahceAsync(entry);
            }
            return entry;
        }

        public abstract Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input);

        public abstract Task<long> SetInCahceAsync(IDictionary<TIdentity, TEntity> pairs);

        protected virtual TimeSpan? GetCacheTime(TIdentity identity, TEntity entity)
        {
            return DataFinderConst.DefaultCacheTime;
        }
        protected virtual bool CanRenewal(TIdentity identity, TEntity entity)
        {
            return true;
        }

        protected abstract Task<IDictionary<TIdentity,TEntity>> OnFindInDbAsync(IReadOnlyList<TIdentity> identities);

        Task<long> IBatchDataFinder.DeleteAsync(IList identity)
        {
            return DeleteAsync(identity.Cast<TIdentity>().ToList());
        }

        async Task<IDictionary<object, bool>> IBatchDataFinder.ExistsAsync(IList identity)
        {
            var res = await ExistsAsync(identity.Cast<TIdentity>().ToList());

            return res.ToDictionary(x => (object)x, x => x.Value);
        }

        Task<long> IBatchCacheFinder.SetInCahceAsync(IDictionary pairs)
        {
            var m = new Dictionary<TIdentity, TEntity>(pairs.Count);
            var enu = pairs.GetEnumerator();
            while (enu.MoveNext())
            {
                m[(TIdentity)enu.Key] = (TEntity)enu.Value;
            }
            return SetInCahceAsync(m);
        }

        async Task<IDictionary> IBatchCacheFinder.FindInCahceAsync(IList identity)
        {
            var res = await FindInCahceAsync(identity.Cast<TIdentity>().ToList());
            if (res is IDictionary map)
            {
                return map;
            }
            return res.ToDictionary(x => x.Key, x => x.Value);
        }

        async Task<IDictionary> IBatchPhysicalFinder.FindInDbAsync(IList identity, bool cache)
        {
            var res = await FindInDbAsync(identity.Cast<TIdentity>().ToList(),cache);
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
    }

}
