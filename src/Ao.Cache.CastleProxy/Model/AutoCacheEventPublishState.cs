using Ao.Cache.CastleProxy.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Model
{
    public class AutoCacheEventPublishState<T>
    {
        public AutoCacheEventPublishTypes Type { get; set; }

        public T Data { get; set; }
    }
}
