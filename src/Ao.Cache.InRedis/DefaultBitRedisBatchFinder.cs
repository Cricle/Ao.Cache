using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class DefaultBitRedisBatchFinder<TIdentity, TEntity> : BitRedisBatchFinder<TIdentity, TEntity>
    {
        public DefaultBitRedisBatchFinder(IDatabase database,
            IBatchDataAccesstor<TIdentity, TEntity> dataAccesstor,
            IEntityConvertor entityConvertor)
            : base(entityConvertor)
        {
            if (entityConvertor is null)
            {
                throw new ArgumentNullException(nameof(entityConvertor));
            }

            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }
        public IDatabase Database { get; }

        public IBatchDataAccesstor<TIdentity, TEntity> DataAccesstor { get; }

        public override IDatabase GetDatabase()
        {
            return Database;
        }

        protected override Task<IDictionary<TIdentity, TEntity>> OnFindInDbAsync(IReadOnlyList<TIdentity> identities)
        {
            return DataAccesstor.FindAsync(identities);
        }
    }
}
