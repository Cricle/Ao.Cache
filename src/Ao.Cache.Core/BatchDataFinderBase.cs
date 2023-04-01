using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class BatchDataFinderBase<TIdentity, TEntity> : OptionalDataFinder<TIdentity, TEntity>, IIdentityGenerater<TIdentity>, IBatchDataFinder<TIdentity, TEntity>, IBatchRenewalable<TIdentity>, IDataFinderOptions<TIdentity, TEntity>
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

        public Task<long> RenewalAsync(IReadOnlyList<TIdentity> input)
        {
            var map = new Dictionary<TIdentity, TimeSpan?>(input.Count);
            foreach (var item in input)
            {
                var time = GetCacheTime(item);
                map[item] = time;
            }
            return RenewalAsync(map);
        }

    }

}
