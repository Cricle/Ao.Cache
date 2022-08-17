using Ao.Cache.CastleProxy.Model;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Annotations
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
    public class AutoCacheInvokeDecoratorContext<TResult>
    {
        public AutoCacheInvokeDecoratorContext(IInvocation invocation, IInvocationProceedInfo invocationProceedInfo, IServiceScopeFactory serviceScopeFactory, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            Invocation = invocation;
            InvocationProceedInfo = invocationProceedInfo;
            ServiceScopeFactory = serviceScopeFactory;
            Proceed = proceed;
            Feature = new Dictionary<object, object>(0);
        }

        public IInvocation Invocation { get; }

        public IInvocationProceedInfo InvocationProceedInfo { get; }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public Func<IInvocation, IInvocationProceedInfo, Task<TResult>> Proceed { get; }

        public IDictionary Feature { get; }
    }
}
