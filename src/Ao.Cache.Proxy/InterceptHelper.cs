using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Interceptors;
using Ao.Cache.Proxy.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy
{
    public struct InterceptLayout
    {
        public InterceptLayout(IServiceScopeFactory serviceScopeFactory, ICacheNamedHelper namedHelper)
        {
            ServiceScopeFactory = serviceScopeFactory;
            NamedHelper = namedHelper;
        }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public ICacheNamedHelper NamedHelper { get; }

        public bool HasAutoCache(IInvocationInfo invocationInfo)
        {
            return AutoCacheAssertions.HasAutoCache(invocationInfo.TargetType) ||
                 AutoCacheAssertions.HasAutoCache(invocationInfo.Method);
        }


        public InterceptToken<TResult> CreateToken<TResult>(IInvocationInfo invocationInfo,IServiceScope scope=null)
        {
            return new InterceptToken<TResult>(invocationInfo, this, scope);
        }

        public async Task<AutoCacheResult<TResult>> RunAsync<TResult>(IInvocationInfo invocationInfo, Func<Task<TResult>> proceed)
        {
            using (var token = CreateToken<TResult>(invocationInfo))
            {
                if (await token.TryFindInCacheAsync() == null)
                {
                    await token.FindInMethodBeginAsync();
                    if (!token.AutoCacheResultBox.HasResult)
                    {
                        try
                        {
                            var res = await proceed();
                            token.AutoCacheResultBox.SetResult(res);
                            await token.FindInMethodEndAsync();
                        }
                        finally
                        {
                            await token.FindInMethodFinallyAsync();
                        }
                    }
                }
                return token.Result;
            }
        }
    }
}
