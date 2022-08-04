using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Model
{
    public interface IAutoCacheResult
    {
        AutoCacheStatus Status { get; set; }
    }
    
    public sealed class AutoCacheResult<T> : IAutoCacheResult
    {
        public T RawData { get; set; }

        public AutoCacheStatus Status {get; set; }
    }
    [Flags]
    public enum AutoCacheStatus
    {
        Skip = 0,
        MethodHit = 1,
        CacheHit = 2
    }
}
