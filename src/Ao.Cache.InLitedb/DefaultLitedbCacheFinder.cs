using Ao.Cache.InLitedb.Models;
using LiteDB;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb
{
    public class DefaultSyncLitedbCacheFinder<TIdentity, TEntry> : LitedbCacheFinder<TIdentity, TEntry>, ISyncWithDataFinder<TIdentity, TEntry>
    {
        public DefaultSyncLitedbCacheFinder(ILiteDatabase database,
            ILiteCollection<LiteCacheEntity> collection,
            IEntityConvertor entityConvertor,
            ISyncDataAccesstor<TIdentity, TEntry> dataAccesstor)
            : base(database, collection, entityConvertor)
        {
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public ISyncDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        public TEntry FindInDb(TIdentity identity, bool cache)
        {
            var entity = DataAccesstor.Find(identity);
            if (cache && entity != null)
            {
                SetInCache(identity, entity);
            }
            return entity;
        }

    }
    public class DefaultLitedbCacheFinder<TIdentity, TEntry> : LitedbCacheFinder<TIdentity, TEntry>, IWithDataFinder<TIdentity, TEntry>
    {
        public DefaultLitedbCacheFinder(ILiteDatabase database,
            ILiteCollection<LiteCacheEntity> collection,
            IEntityConvertor entityConvertor,
            IDataAccesstor<TIdentity, TEntry> dataAccesstor)
            : base(database, collection, entityConvertor)
        {
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
        }

        public IDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        public async Task<TEntry> FindInDbAsync(TIdentity identity, bool cache)
        {
            var entity = await DataAccesstor.FindAsync(identity);
            if (cache&&entity!=null)
            {
                await SetInCacheAsync(identity, entity);
            }
            return entity;
        }

    }
}
