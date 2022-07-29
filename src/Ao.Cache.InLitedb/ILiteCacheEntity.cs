using LiteDB;
using System;

namespace Ao.Cache.InLitedb
{
    public interface ILiteCacheEntity
    {
        ObjectId Id { get; }

        DateTime? ExpirationTime { get; set; }
    }
}
