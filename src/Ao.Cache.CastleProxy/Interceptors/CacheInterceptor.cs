using Ao.Cache.Proxy;
using Ao.Cache.Proxy.Model;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public class CacheInterceptor : AsyncInterceptorBase, IInterceptor
    {
        public CacheInterceptor(IServiceScopeFactory serviceScopeFactory,
            IStringTransfer stringTransfer,
            ICacheNamedHelper cacheNamedHelper)
        {
            ServiceScopeFactory = serviceScopeFactory;
            StringTransfer = stringTransfer;
            NamedHelper = cacheNamedHelper;
        }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public IStringTransfer StringTransfer { get; }

        public ICacheNamedHelper NamedHelper { get; }

        protected override Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            return proceed(invocation, proceedInfo);
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var ii = new InvocationInfo(invocation);
            var layout = new InterceptLayout(ServiceScopeFactory, NamedHelper);
            if (!InterceptLayout.HasAutoCache(ii))
            {
                var res = await proceed(invocation, proceedInfo).ConfigureAwait(false);
                if (res is IAutoCacheResult result)
                {
                    result.Status = AutoCacheStatus.Skip;
                }
                return res;
            }
            using (var token = layout.CreateToken<TResult>(ii))
            {
                try
                {
                    await token.InterceptBeginAsync();
                    var res = await token.TryFindInCacheAsync();
                    if (res == null)
                    {
                        await token.FindInMethodBeginAsync();
                        if (!token.AutoCacheResultBox.HasResult)
                        {
                            try
                            {
                                res = await proceed(invocation, proceedInfo);
                                token.AutoCacheResultBox.SetResult(res);
                                await token.FindInMethodEndAsync();
                            }
                            finally
                            {
                                await token.FindInMethodFinallyAsync();
                            }
                        }
                    }
                    else
                    {
                        token.Result.RawData = res;
                        token.AutoCacheResultBox.SetResult(res);
                    }
                }
                catch (Exception ex)
                {
                    await token.InterceptExceptionAsync(ex);
                    throw;
                }
                finally
                {
                    await token.FinallyAsync();

                }
                if (CacheResultNewExpression<TResult>.IsAutoResult)
                {
                    var dyn = (IAutoCacheResult)CacheResultNewExpression<TResult>.Creator();
                    dyn.Status = token.Result.Status;
                    var d = token.AutoCacheResultBox.Result;
                    if (d != null)
                    {
                        dyn.RawData = ((IAutoCacheResult)d).RawData;
                    }
                    return (TResult)dyn;
                }
                return token.AutoCacheResultBox.Result;
            }
        }

        public void Intercept(IInvocation invocation)
        {
            InterceptSynchronous(invocation);
        }
    }
}
