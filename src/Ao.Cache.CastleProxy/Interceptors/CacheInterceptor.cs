using Ao.Cache.Proxy;
using Ao.Cache.Proxy.Model;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
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
            if (!layout.HasAutoCache(ii))
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
                    if (await token.TryFindInCacheAsync() == null)
                    {
                        await token.FindInMethodBeginAsync();
                        if (!token.AutoCacheResultBox.HasResult)
                        {
                            try
                            {
                                var res = await proceed(invocation, proceedInfo);
                                token.AutoCacheResultBox.SetResult(res);
                                await token.FindInMethodEndAsync();
                            }
                            finally
                            {
                                await token.FindInMethodFinallyAsync();
                            }
                        }
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
                if (NewExpression<TResult>.IsAutoResult)
                {
                    dynamic dyn = NewExpression<TResult>.Creator();
                    dyn.Status = token.Result.Status;
                    var d = token.AutoCacheResultBox.Result;
                    if (d != null)
                    {
                        dyn.RawData = ((dynamic)token.AutoCacheResultBox.Result).RawData;
                    }
                    return dyn;
                }
                return token.Result.RawData;
            }
        }

        static class NewExpression<T>
        {
            public static readonly Func<T> Creator;

            public static bool IsAutoResult;

            static NewExpression()
            {
                var typeT = typeof(T);
                IsAutoResult = typeT.IsGenericType &&
                    typeT.GetGenericTypeDefinition() == typeof(AutoCacheResult<>);
                Creator = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
            }
        }

        public void Intercept(IInvocation invocation)
        {
            InterceptSynchronous(invocation);
        }
    }
}
