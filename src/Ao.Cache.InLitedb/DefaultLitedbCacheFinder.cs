using Ao.Cache.InLitedb.Models;
using LiteDB;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb
{
    public class DefaultLitedbCacheFinder<TIdentity, TEntry> : LitedbCacheFinder<TIdentity, TEntry>,IWithDataAccesstorFinder<TIdentity,TEntry>
    {
        public DefaultLitedbCacheFinder(ILiteDatabase database,
            ILiteCollection<LiteCacheEntity> collection,
            IEntityConvertor entityConvertor,
            IDataAccesstor<TIdentity, TEntry> dataAccesstor) 
            : base(database,collection, entityConvertor)
        {
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        protected override Task<TEntry> OnFindInDbAsync(TIdentity identity)
        {
            return DataAccesstor.FindAsync(identity);
        }
    }
}
