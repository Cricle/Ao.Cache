using System;
using System.Collections;
using System.Collections.Generic;

namespace Ao.Cache.Proxy.Annotations
{
    public class AutoCacheDecoratorContext<TResult>
    {
        public AutoCacheDecoratorContext(IInvocationInfo invocationInfo, IServiceProvider serviceProvider, IDataFinder<UnwindObject, TResult> dataFinder, UnwindObject identity)
        {
            InvocationInfo = invocationInfo;
            ServiceProvider = serviceProvider;
            DataFinder = dataFinder;
            Identity = identity;
            Features = new Dictionary<object, object>(0);
        }

        public IInvocationInfo InvocationInfo { get; }

        public IServiceProvider ServiceProvider { get; }

        public IDataFinder<UnwindObject, TResult> DataFinder { get; }

        public UnwindObject Identity { get; }

        public IDictionary Features { get; }
    }
}
