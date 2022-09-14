using Ao.Cache.Proxy.Annotations;

namespace Ao.Cache.Proxy.Model
{
    public class AutoCacheEventPublishState<T>
    {
        public AutoCacheEventPublishTypes Type { get; set; }

        public T Data { get; set; }
    }
}
