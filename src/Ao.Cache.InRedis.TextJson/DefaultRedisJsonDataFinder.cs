using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis.TextJson
{
    public class DefaultRedisJsonDataFinder<TIdentity, TEntry> : BitRedisDataFinder<TIdentity, TEntry>, IWithDataAccesstorFinder<TIdentity, TEntry>
    {
        public DefaultRedisJsonDataFinder(IDatabase database,
            IDataAccesstor<TIdentity, TEntry> dataAccesstor)
            : this(database, dataAccesstor, TextJsonEntityConvertor<TEntry>.Default)
        {
        }
        public DefaultRedisJsonDataFinder(IDatabase database, 
            IDataAccesstor<TIdentity, TEntry> dataAccesstor,
            IEntityConvertor<TEntry> entityConvertor)
            :base(entityConvertor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IDatabase Database { get; }

        public IDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        public override IDatabase GetDatabase()
        {
            return Database;
        }

        protected override Task<TEntry> OnFindInDbAsync(TIdentity identity)
        {
            return DataAccesstor.FindAsync(identity);
        }
    }

}
