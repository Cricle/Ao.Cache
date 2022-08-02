using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Model
{
    public interface IAutoCacheResultBase
    {
        AutoCacheStatus Status { get; set; }
    }
    public class AutoCacheResult<T> : IAutoCacheResultBase
    {
        public T RawData { get; set; }

        public AutoCacheStatus Status {get; set; }
    }
    [Flags]
    public enum AutoCacheStatus
    {
        Skip = 0,
        MethodHit = 1,
        CacheHit = 2,
        NotSupportFinderOrAccesstor = 3
    }
}
