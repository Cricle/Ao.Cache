using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.TextJson.Redis
{
    public class DefaultRedisJsonDataFinder<TIdentity, TEntry> : RedisJsonDataFinder<TIdentity, TEntry>
    {
        public DefaultRedisJsonDataFinder(IDatabase database, IDataAccesstor<TIdentity, TEntry> dataAccesstor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IDatabase Database { get; }

        public IDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        protected override IDatabase GetDatabase()
        {
            return Database;
        }

        protected override Task<TEntry> OnFindInDbAsync(TIdentity identity)
        {
            return DataAccesstor.FindAsync(identity);
        }
        protected override TimeSpan? GetCacheTime(TIdentity identity, TEntry entity)
        {
            return DataAccesstor.GetCacheTime(identity, entity);
        }

        public override string GetHead()
        {
            return DataAccesstor.GetHead() ?? base.GetHead();
        }
        public override string GetPart(TIdentity identity)
        {
            return DataAccesstor.GetPart(identity);
        }
        protected override bool CanRenewal(TIdentity identity, TEntry entity)
        {
            return DataAccesstor.CanRenewal(identity);
        }

    }

}
