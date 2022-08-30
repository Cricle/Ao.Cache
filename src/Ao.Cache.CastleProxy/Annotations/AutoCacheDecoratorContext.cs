using Castle.DynamicProxy;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ao.Cache.CastleProxy.Annotations
{
    public class AutoCacheDecoratorContext<TResult>
    {
        public AutoCacheDecoratorContext(IInvocation invocation, IInvocationProceedInfo invocationProceedInfo, IServiceProvider serviceProvider, IDataFinder<UnwindObject, TResult> dataFinder, UnwindObject identity)
        {
            Invocation = invocation;
            InvocationProceedInfo = invocationProceedInfo;
            ServiceProvider = serviceProvider;
            DataFinder = dataFinder;
            Identity = identity;
            Features = new Dictionary<object, object>(0);
        }

        public IInvocation Invocation { get; }

        public IInvocationProceedInfo InvocationProceedInfo { get; }

        public IServiceProvider ServiceProvider { get; }

        public IDataFinder<UnwindObject, TResult> DataFinder { get; }

        public UnwindObject Identity { get; }

        public IDictionary Features { get; }
    }
}
