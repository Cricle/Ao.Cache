using Castle.DynamicProxy;
using System;

namespace Ao.Cache.CastleProxy.Annotations
{
    public readonly struct AutoCacheDecoratorContext<TResult>
    {
        public IInvocation Invocation { get; }

        public IInvocationProceedInfo InvocationProceedInfo { get; }

        public IServiceProvider ServiceProvider { get; }

        public IDataFinder<UnwindObject, TResult> DataFinder { get; }

        public UnwindObject Identity { get; }

        public AutoCacheDecoratorContext(IInvocation invocation, IInvocationProceedInfo invocationProceedInfo, IServiceProvider serviceProvider, IDataFinder<UnwindObject, TResult> dataFinder, UnwindObject identity)
        {
            Invocation = invocation;
            InvocationProceedInfo = invocationProceedInfo;
            ServiceProvider = serviceProvider;
            DataFinder = dataFinder;
            Identity = identity;
        }
    }
}
