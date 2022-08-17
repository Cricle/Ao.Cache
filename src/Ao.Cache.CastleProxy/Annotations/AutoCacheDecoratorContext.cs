using Ao.Cache.CastleProxy.Model;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

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
        }

        public IInvocation Invocation { get; }

        public IInvocationProceedInfo InvocationProceedInfo { get; }

        public IServiceProvider ServiceProvider { get; }

        public IDataFinder<UnwindObject, TResult> DataFinder { get; }

        public UnwindObject Identity { get; }

    }
}
