using System;

namespace Ao.Cache.InLitedb
{
    public interface ILiteCacheEntity
    {
        DateTime? ExpirationTime { get; set; }
    }
}
