using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis.HashList.Finders
{
    public class DefaultListCacheFinder<TIdentity, TEntry> : ListCacheFinder<TIdentity, TEntry>
    {
        public DefaultListCacheFinder(IDatabase database, IDataAccesstor<TIdentity, TEntry> dataAccesstor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
            Build();
        }

        public IDatabase Database { get; }

        public IDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        protected override Task<TEntry> OnFindInDbAsync(TIdentity identity)
        {
            return DataAccesstor.FindAsync(identity);
        }
        public override IDatabase GetDatabase()
        {
            return Database;
        }
    }

}