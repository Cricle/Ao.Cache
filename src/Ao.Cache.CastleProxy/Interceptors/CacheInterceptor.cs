using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Model;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public class CacheInterceptor : NamedInterceptor
    {
        private static readonly object locker = new object();
        private static readonly Dictionary<NamedInterceptorKey, NamedInterceptorValue> argCacheMap = new Dictionary<NamedInterceptorKey, NamedInterceptorValue>();
        public CacheInterceptor(IServiceScopeFactory serviceScopeFactory, IStringTransfer stringTransfer)
        {
            ServiceScopeFactory = serviceScopeFactory;
            StringTransfer = stringTransfer;
        }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public IStringTransfer StringTransfer { get; }

        protected override Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            return proceed(invocation, proceedInfo);
        }
        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            if (GetAutoCache(invocation) == null)
            {
                var res = await proceed(invocation, proceedInfo);
                if (res is IAutoCacheResultBase result)
                {
                    result.Status = AutoCacheStatus.Skip;
                }
                return res;
            }
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var finder = scope.ServiceProvider.GetRequiredService<IDataFinder<UnwindObject, TResult>>();
                if (finder is IWithDataAccesstorFinder<UnwindObject, TResult> f && f.DataAccesstor is CastleDataAccesstor<UnwindObject, TResult> unwinAccsstor)
                {
                    var key = new NamedInterceptorKey(invocation.TargetType, invocation.Method);
                    var lst = GetArgIndexs(key, invocation.Method);
                    var args = MakeArgsWithHeader(lst, invocation.Arguments);
                    var winObj = new UnwindObject(args, StringTransfer);
                    var res = await finder.FindInCahceAsync(winObj);
                    if (res == null)
                    {
                        res = await proceed(invocation, proceedInfo);
                        unwinAccsstor.Invocation = invocation;
                        unwinAccsstor.ProceedInfo = proceedInfo;
                        unwinAccsstor.Proceed = proceed;
                        await finder.SetInCahceAsync(winObj, res);
                        if (res is IAutoCacheResultBase result)
                        {
                            result.Status = AutoCacheStatus.MethodHit;
                        }
                        return res;
                    }
                    if (res is IAutoCacheResultBase resultr)
                    {
                        resultr.Status = AutoCacheStatus.CacheHit;
                    }
                    invocation.ReturnValue = res;
                    return res;
                }
                else
                {
                    var res = await proceed(invocation, proceedInfo);
                    if (res is IAutoCacheResultBase result)
                    {
                        result.Status = AutoCacheStatus.NotSupportFinderOrAccesstor;
                    }
                    return res;
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AutoCacheAttribute GetAutoCache(IInvocation invocation)
        {
            return invocation.TargetType.GetCustomAttribute<AutoCacheAttribute>() ?? invocation.Method.GetCustomAttribute<AutoCacheAttribute>();
        }

        protected override Dictionary<NamedInterceptorKey, NamedInterceptorValue> GetCacheMap()
        {
            return argCacheMap;
        }

        protected override bool ParamterCanUse(ParameterInfo param)
        {
            return param.GetCustomAttribute<AutoCacheSkipPartAttribute>() == null;
        }

        protected override object GetLocker()
        {
            return locker;
        }
    }
}
