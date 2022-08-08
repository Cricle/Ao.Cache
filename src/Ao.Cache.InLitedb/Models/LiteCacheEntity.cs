using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb.Models
{
    public class LiteCacheEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Identity { get; set; }

        public byte[] Data { get; set; }

        public DateTime? ExpireTime { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
