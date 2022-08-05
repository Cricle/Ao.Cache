using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public class MixedDataFinder<TIdentity, TEntity>:IMixedDataFinder<TIdentity, TEntity>
    {
        public MixedDataFinder(IDataFinder<TIdentity, TEntity> dataFinder, IBatchDataFinder<TIdentity, TEntity> batchDataFinder)
        {
            DataFinder = dataFinder ?? throw new System.ArgumentNullException(nameof(dataFinder));
            BatchDataFinder = batchDataFinder ?? throw new System.ArgumentNullException(nameof(batchDataFinder));
        }

        public IDataFinder<TIdentity,TEntity> DataFinder { get; }

        public IBatchDataFinder<TIdentity, TEntity> BatchDataFinder { get; }

        public Task<bool> DeleteAsync(TIdentity identity)
        {
            return DataFinder.DeleteAsync(identity);
        }

        public Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity)
        {
            return BatchDataFinder.DeleteAsync(identity);
        }

        public Task<bool> ExistsAsync(TIdentity identity)
        {
            return DataFinder.ExistsAsync(identity);
        }

        public Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity)
        {
            return BatchDataFinder.ExistsAsync(identity);
        }

        public Task<TEntity> FindInCahceAsync(TIdentity identity)
        {
            return DataFinder.FindInCahceAsync(identity);
        }

        public Task<IDictionary<TIdentity, TEntity>> FindInCahceAsync(IReadOnlyList<TIdentity> identity)
        {
            return BatchDataFinder.FindInCahceAsync(identity);
        }

        public Task<TEntity> FindInDbAsync(TIdentity identity, bool cache)
        {
            return DataFinder.FindInDbAsync(identity,cache);
        }

        public Task<IDictionary<TIdentity, TEntity>> FindInDbAsync(IReadOnlyList<TIdentity> identity, bool cache)
        {
            return BatchDataFinder.FindInDbAsync(identity,cache);
        }

        public Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time)
        {
            return DataFinder.RenewalAsync(identity, time);
        }

        public Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input)
        {
            return BatchDataFinder.RenewalAsync(input);
        }

        public Task<bool> SetInCahceAsync(TIdentity identity, TEntity entity)
        {
            return DataFinder.SetInCahceAsync(identity, entity);
        }

        public Task<long> SetInCahceAsync(IDictionary<TIdentity, TEntity> pairs)
        {
            return BatchDataFinder.SetInCahceAsync(pairs);
        }
    }
}
