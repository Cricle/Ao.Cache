using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public abstract class BatchDataFinderBase<TIdentity, TEntity> : IdentityGenerater<TIdentity,TEntity>, IBatchDataFinder<TIdentity, TEntity>, IBatchRenewalable<TIdentity>
    {
        public abstract Task<IDictionary<TIdentity, bool>> DeleteAsync(IReadOnlyList<TIdentity> identity);
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

        public abstract Task<IDictionary<TIdentity, bool>> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input);

        public abstract Task<IDictionary<TIdentity, bool>> SetInCahceAsync(IDictionary<TIdentity, TEntity> pairs);

        protected virtual bool CanRenewal(TIdentity identity, TEntity entity)
        {
            return true;
        }

        protected abstract Task<IDictionary<TIdentity,TEntity>> OnFindInDbAsync(IReadOnlyList<TIdentity> identities);

    }

}
