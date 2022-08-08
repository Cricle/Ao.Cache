using LiteDB;
using System;

namespace Ao.Cache.InLitedb.Models
{
    public class LiteLockEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Resource { get; set; }

        public DateTime ExpireTime { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
