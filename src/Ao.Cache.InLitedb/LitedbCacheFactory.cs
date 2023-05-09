using Ao.Cache.InLitedb.Models;
using LiteDB;
using System;

namespace Ao.Cache.InLitedb
{
    public class LitedbCacheFactory : IDataFinderFactory, IBatchDataFinderFactory, ISyncDataFinderFactory, ISyncBatchDataFinderFactory
    {
        public LitedbCacheFactory(ILiteDatabase database, ILiteCollection<LiteCacheEntity> collection, IEntityConvertor entityConvertor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
            EntityConvertor = entityConvertor ?? throw new ArgumentNullException(nameof(entityConvertor));
        }

        public ILiteDatabase Database { get; }

        public ILiteCollection<LiteCacheEntity> Collection { get; }

        public IEntityConvertor EntityConvertor { get; }

        public IWithDataFinder<TIdentity, TEntry> Create<TIdentity, TEntry>(IDataAccesstor<TIdentity, TEntry> accesstor)
        {
            return new DefaultLitedbCacheFinder<TIdentity, TEntry>(Database, Collection, EntityConvertor, accesstor);
        }

        public IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>()
        {
            return new LitedbCacheFinder<TIdentity, TEntity>(Database, Collection, EntityConvertor);
        }

        public ISyncWithDataFinder<TIdentity, TEntity> CreateSync<TIdentity, TEntity>(ISyncDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultSyncLitedbCacheFinder<TIdentity, TEntity>(Database, Collection, EntityConvertor, accesstor);
        }

        public IWithBatchDataFinder<TIdentity, TEntry> CreateBatch<TIdentity, TEntry>(IBatchDataAccesstor<TIdentity, TEntry> accesstor)
        {
            return new DefaultBatchLitedbCacheFinder<TIdentity, TEntry>(Database, Collection, EntityConvertor, accesstor);
        }

        public IBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>()
        {
            return new LitedbBatchCacheFinder<TIdentity, TEntity>(Database, Collection, EntityConvertor);
        }

        public ISyncWithBatchDataFinder<TIdentity, TEntity> CreateBatchSync<TIdentity, TEntity>(ISyncBatchDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DefaultSyncBatchLitedbCacheFinder<TIdentity, TEntity>(Database, Collection, EntityConvertor, accesstor);
        }

        public ISyncDataFinder<TIdentity, TEntity> CreateSync<TIdentity, TEntity>()
        {
            return new LitedbCacheFinder<TIdentity, TEntity>(Database, Collection, EntityConvertor);
        }

        public ISyncBatchDataFinder<TIdentity, TEntity> CreateBatchSync<TIdentity, TEntity>()
        {
            return new LitedbBatchCacheFinder<TIdentity, TEntity>(Database, Collection, EntityConvertor);
        }
    }
}
