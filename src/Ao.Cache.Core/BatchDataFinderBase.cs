using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class BatchDataFinderBase<TIdentity, TEntity> : OptionalDataFinder<TIdentity, TEntity>, IIdentityGenerater<TIdentity>, IBatchDataFinder<TIdentity, TEntity>, ISyncBatchDataFinder<TIdentity,TEntity>, IDataFinderOptions<TIdentity, TEntity>
    {
        public abstract Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity);
        public abstract Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity);

        public async Task<IDictionary<TIdentity, TEntity>> FindInCacheAsync(IReadOnlyList<TIdentity> identity)
        {
            var entity = await CoreFindInCacheAsync(identity);
            if (entity.Count != 0)
            {
                var renewals = entity.Keys.Where(x => CanRenewal(x)).ToList();
                if (renewals.Count != 0)
                {
                    await RenewalAsync(renewals);
                }
            }
            return entity;
        }

        public virtual async Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IBatchDataAccesstor<TIdentity, TEntity> batchDataAccesstor, IReadOnlyList<TIdentity> identity, bool cache)
        {
            var entry = await batchDataAccesstor.FindAsync(identity);
            if (entry != null && cache)
            {
                await SetInCacheAsync(entry);
            }
            return entry;
        }

        protected abstract Task<IDictionary<TIdentity, TEntity>> CoreFindInCacheAsync(IReadOnlyList<TIdentity> identity);

        public abstract Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input);

        public abstract Task<long> SetInCacheAsync(IDictionary<TIdentity, TEntity> pairs);

        public virtual Task<long> RenewalAsync(IReadOnlyList<TIdentity> input)
        {
            var map = new Dictionary<TIdentity, TimeSpan?>(input.Count);
            foreach (var item in input)
            {
                var time = GetCacheTime(item);
                map[item] = time;
            }
            return RenewalAsync(map);
        }

        public abstract long Delete(IReadOnlyList<TIdentity> identity);

        public abstract IDictionary<TIdentity, bool> Exists(IReadOnlyList<TIdentity> identity);

        public abstract long SetInCache(IDictionary<TIdentity, TEntity> pairs);

        protected abstract IDictionary<TIdentity, TEntity> CoreFindInCache(IReadOnlyList<TIdentity> identity);

        public IDictionary<TIdentity, TEntity> FindInCache(IReadOnlyList<TIdentity> identity)
        {
            var entity = CoreFindInCache(identity);
            if (entity.Count != 0)
            {
                var renewals = entity.Keys.Where(x => CanRenewal(x)).ToList();
                if (renewals.Count != 0)
                {
                    Renewal(renewals);
                }
            }
            return entity;
        }

        public IDictionary<TIdentity, TEntity> FindInDb(ISyncBatchDataAccesstor<TIdentity, TEntity> batchDataAccesstor, IReadOnlyList<TIdentity> identity, bool cache)
        {
            var entry = batchDataAccesstor.Find(identity);
            if (entry != null && cache)
            {
                SetInCache(entry);
            }
            return entry;
        }

        public virtual long Renewal(IReadOnlyList<TIdentity> input)
        {
            var map = new Dictionary<TIdentity, TimeSpan?>(input.Count);
            foreach (var item in input)
            {
                var time = GetCacheTime(item);
                map[item] = time;
            }
            return Renewal(map);
        }

        public abstract long Renewal(IDictionary<TIdentity, TimeSpan?> input);
    }

}
