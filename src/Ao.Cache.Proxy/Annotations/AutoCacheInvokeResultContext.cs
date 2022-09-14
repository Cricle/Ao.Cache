using Ao.Cache.Proxy.Model;
using System;

namespace Ao.Cache.Proxy.Annotations
{
    public class AutoCacheInvokeResultContext<TResult>
    {
        public AutoCacheInvokeResultContext(TResult result, IAutoCacheResult cacheResult, Exception exception)
        {
            Result = result;
            CacheResult = cacheResult;
            Exception = exception;
        }

        public TResult Result { get; }

        public IAutoCacheResult CacheResult { get; }

        public Exception Exception { get; }
    }
}
