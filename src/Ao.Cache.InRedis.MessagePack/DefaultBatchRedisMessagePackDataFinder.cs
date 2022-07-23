using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis.MessagePack
{
    public class DefaultBatchRedisMessagePackDataFinder<TIdentity, TEntry> : BitRedisBatchFinder<TIdentity, TEntry>, IWithBatchDataAccesstorFinder<TIdentity, TEntry>
    {
        public DefaultBatchRedisMessagePackDataFinder(IDatabase database,
            IBatchDataAccesstor<TIdentity, TEntry> dataAccesstor)
            : this(database, dataAccesstor, MessagePackEntityConvertor<TEntry>.Default)
        {
        }
        public DefaultBatchRedisMessagePackDataFinder(IDatabase database,
            IBatchDataAccesstor<TIdentity, TEntry> dataAccesstor,
            IEntityConvertor<TEntry> entityConvertor)
            : base(entityConvertor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IDatabase Database { get; }

        public IBatchDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        public override IDatabase GetDatabase()
        {
            return Database;
        }

        protected override Task<IDictionary<TIdentity, TEntry>> OnFindInDbAsync(IReadOnlyList<TIdentity> identities)
        {
            return DataAccesstor.FindAsync(identities);
        }
    }
}
