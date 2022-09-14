using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;

namespace Ao.Cache.Proxy.Annotations
{
    public class AutoCacheInvokeDecoratorContext<TResult>
    {
        public AutoCacheInvokeDecoratorContext(IInvocationInfo invocationInfo, IServiceScopeFactory serviceScopeFactory)
        {
            InvocationInfo = invocationInfo;
            ServiceScopeFactory = serviceScopeFactory;
        }

        private IDictionary feature;

        public IInvocationInfo InvocationInfo { get; }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public IDictionary Feature
        {
            get
            {
                if (feature == null)
                {
                    feature = new Dictionary<object, object>();
                }
                return feature;
            }
        }
    }
}
