using Ao.Cache.InLitedb.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb
{
    public class DefaultSyncBatchLitedbCacheFinder<TIdentity, TEntry> : LitedbBatchCacheFinder<TIdentity, TEntry>, ISyncWithBatchDataFinder<TIdentity, TEntry>
    {
        public DefaultSyncBatchLitedbCacheFinder(ILiteDatabase database,
            ILiteCollection<LiteCacheEntity> collection,
            IEntityConvertor entityConvertor,
            ISyncBatchDataAccesstor<TIdentity, TEntry> dataAccesstor)
            : base(database, collection, entityConvertor)
        {
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public ISyncBatchDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        public IDictionary<TIdentity, TEntry> FindInDb(IReadOnlyList<TIdentity> identity, bool cache)
        {
            var entity = DataAccesstor.Find(identity);
            if (cache && entity != null)
            {
                SetInCache(entity);
            }
            return entity;
        }

    }
    public class DefaultBatchLitedbCacheFinder<TIdentity, TEntry> : LitedbBatchCacheFinder<TIdentity, TEntry>, IWithBatchDataFinder<TIdentity, TEntry>
    {
        public DefaultBatchLitedbCacheFinder(ILiteDatabase database,
            ILiteCollection<LiteCacheEntity> collection,
            IEntityConvertor entityConvertor,
            IBatchDataAccesstor<TIdentity, TEntry> dataAccesstor)
            : base(database, collection, entityConvertor)
        {
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IBatchDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        public async Task<IDictionary<TIdentity, TEntry>> FindInDbAsync(IReadOnlyList<TIdentity> identity, bool cache)
        {
            var entity = await DataAccesstor.FindAsync(identity);
            if (cache&&entity!=null)
            {
                await SetInCacheAsync(entity);
            }
            return entity;
        }

    }
}
