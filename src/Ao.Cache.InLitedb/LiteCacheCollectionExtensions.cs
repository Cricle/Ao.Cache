using Ao.Cache.InLitedb.Models;

namespace LiteDB
{
    public static class LiteCacheCollectionExtensions
    {
        public static ILiteCollection<LiteCacheEntity> GetCacheCollection(this ILiteDatabase db, BsonAutoId id= BsonAutoId.ObjectId)
        {
            return db.GetCollection<LiteCacheEntity>(id);
        }
        public static ILiteCollection<LiteCacheEntity> GetCacheCollection(this ILiteDatabase db,string name, BsonAutoId id = BsonAutoId.ObjectId)
        {
            return db.GetCollection<LiteCacheEntity>(name, id);
        }
        public static void EnsureIndex(this ILiteCollection<LiteCacheEntity> coll)
        {
            coll.EnsureIndex(x => x.Identity);
            coll.EnsureIndex(x => x.ExpireTime);
        }
    }
}
