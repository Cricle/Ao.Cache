using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Interceptors;
using Ao.Cache.Proxy.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy
{
    public class InterceptLayout
    {
        public InterceptLayout(IServiceScopeFactory serviceScopeFactory, ICacheNamedHelper namedHelper)
        {
            ServiceScopeFactory = serviceScopeFactory;
            NamedHelper = namedHelper;
        }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public ICacheNamedHelper NamedHelper { get; }

        public async Task<AutoCacheResult<TResult>> RunAsync<TResult>(IInvocationInfo invocationInfo,Func<Task<TResult>> proceed)
        {
            var rr = new AutoCacheResult<TResult>();
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var finderFactory = scope.ServiceProvider.GetRequiredService<IDataFinderFactory>();
                var finder = finderFactory.Create(new CastleDataAccesstor<UnwindObject, TResult>(proceed));
                var key = new NamedInterceptorKey(invocationInfo.TargetType, invocationInfo.Method);
                var attr = DecoratorHelper.Get(key);

                var winObj = NamedHelper.GetUnwindObject(key, invocationInfo.Arguments);
                var ctx = new AutoCacheDecoratorContext<TResult>(
                    invocationInfo, scope.ServiceProvider, finder, winObj);
                for (int i = 0; i < attr.Length; i++)
                {
                    await attr[i].DecorateAsync(ctx).ConfigureAwait(false);
                }
                var res = await finder.FindInCacheAsync(winObj).ConfigureAwait(false);
                if (res == null)
                {
                    var resultBox = new AutoCacheResultBox<TResult>();
                    for (int i = 0; i < attr.Length; i++)
                    {
                        await attr[i].FindInMethodBeginAsync(ctx, resultBox).ConfigureAwait(false);
                    }
                    try
                    {
                        if (resultBox.HasResult)
                        {
                            res = resultBox.Result;
                            rr.Status = AutoCacheStatus.Intercept;
                        }
                        else
                        {
                            res = await proceed().ConfigureAwait(false);
                            await finder.SetInCacheAsync(winObj, res).ConfigureAwait(false);
                            rr.Status = AutoCacheStatus.MethodHit;
                        }
                        rr.RawData = res;
                        for (int i = 0; i < attr.Length; i++)
                        {
                            await attr[i].FindInMethodEndAsync(ctx, res, resultBox.HasResult).ConfigureAwait(false);
                        }
                        return rr;
                    }
                    finally
                    {
                        for (int i = 0; i < attr.Length; i++)
                        {
                            await attr[i].FindInMethodFinallyAsync(ctx).ConfigureAwait(false);
                        }
                    }
                }

                rr.Status = AutoCacheStatus.CacheHit;
                rr.RawData = res;
                invocationInfo.ReturnValue = res;
                for (int i = 0; i < attr.Length; i++)
                {
                    await attr[i].FoundInCacheAsync(ctx, res).ConfigureAwait(false);
                }
                return rr;
            }
        }
    }
}
