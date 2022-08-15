using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class DefaultBitRedisBatchFinder<TIdentity, TEntity> : BitRedisBatchFinder<TIdentity, TEntity>,IWithBatchDataAccesstorFinder<TIdentity,TEntity>
    {
        public DefaultBitRedisBatchFinder(IDatabase database,
            IBatchDataAccesstor<TIdentity, TEntity> dataAccesstor,
            IEntityConvertor entityConvertor)
            : base(database,entityConvertor)
        {
            if (entityConvertor is null)
            {
                throw new ArgumentNullException(nameof(entityConvertor));
            }

            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IBatchDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        protected override Task<IDictionary<TIdentity, TEntity>> OnFindInDbAsync(IReadOnlyList<TIdentity> identities)
        {
            return DataAccesstor.FindAsync(identities);
        }
    }
}
