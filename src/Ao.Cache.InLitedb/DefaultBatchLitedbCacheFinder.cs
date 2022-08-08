using Ao.Cache.InLitedb.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb
{
    public class DefaultBatchLitedbCacheFinder<TIdentity, TEntry> : LitedbBatchCacheFinder<TIdentity, TEntry>, IWithBatchDataAccesstorFinder<TIdentity, TEntry>
    {
        public DefaultBatchLitedbCacheFinder(ILiteDatabase database,
            ILiteCollection<LiteCacheEntity> collection,
            IEntityConvertor entityConvertor,
            IBatchDataAccesstor<TIdentity, TEntry> dataAccesstor)
            : base(database,collection, entityConvertor)
        {
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IBatchDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        protected override Task<IDictionary<TIdentity, TEntry>> OnFindInDbAsync(IReadOnlyList<TIdentity> identities)
        {
            return DataAccesstor.FindAsync(identities);
        }
    }
}
