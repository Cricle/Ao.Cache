using System;
using System.Collections.Generic;
using System.Text;

namespace Ao.Cache.InRedis.HashList
{
    public interface ICacheColumnAnalysis
    {
        IReadOnlyDictionary<string, ICacheColumn> GetRedisColumnMap(Type type, string prefx);

        IReadOnlyList<ICacheColumn> GetRedisColumns(Type type, string prefx);
    }
}
