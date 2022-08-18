using Ao.Cache.CastleProxy.Model;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Annotations
{
    public class AutoCacheResultBox<TResult>
    {
        private bool hasResult;
        private TResult result;

        public TResult Result => result;

        public bool HasResult => hasResult;

        public void SetResult(TResult result)
        {
            hasResult = true;
            this.result = result;
        }
    }
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
