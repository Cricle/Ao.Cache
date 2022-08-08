using Ao.Cache.InLitedb.Models;
using LiteDB;
using System;

namespace Ao.Cache.InLitedb
{
    public class LitedbCacheFactory : IDataFinderFactory, IBatchDataFinderFactory
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

        public IDataFinder<TIdentity, TEntry> Create<TIdentity, TEntry>(IDataAccesstor<TIdentity, TEntry> accesstor)
        {
            return new DefaultLitedbCacheFinder<TIdentity, TEntry>(Database,Collection, EntityConvertor, accesstor);
        }

        public IBatchDataFinder<TIdentity, TEntry> Create<TIdentity, TEntry>(IBatchDataAccesstor<TIdentity, TEntry> accesstor)
        {
            return new DefaultBatchLitedbCacheFinder<TIdentity, TEntry>(Database, Collection, EntityConvertor, accesstor);
        }
    }
}
