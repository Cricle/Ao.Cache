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
    public class CacheInterceptor : AsyncInterceptorBase
    {
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
        private static readonly Dictionary<Tuple<Type, MethodInfo>, List<int>> argCacheMap = new Dictionary<Tuple<Type, MethodInfo>, List<int>>();
        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            if (GetAutoCache(invocation) == null)
            {
                var res = await proceed(invocation, proceedInfo);
                if (res is AutoCacheResultBase result)
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
                    var key = new Tuple<Type, MethodInfo>(invocation.TargetType, invocation.Method);
                    if (!argCacheMap.TryGetValue(key, out var lst))
                    {
                        var methodArgs = invocation.Method.GetParameters();
                        var used = new List<int>();
                        for (int i = 0; i < methodArgs.Length; i++)
                        {
                            var methodArg = methodArgs[i];
                            if (methodArg.GetCustomAttribute<AutoCacheSkipPartAttribute>() == null)
                            {
                                used.Add(i);
                            }
                        }
                        if (used.Count == methodArgs.Length)
                        {
                            argCacheMap[key] = null;
                        }
                        else
                        {
                            argCacheMap[key] = used;
                            lst = used;
                        }
                    }
                    var args = invocation.Arguments;
                    if (lst != null)
                    {
                        args = new object[lst.Count];
                        for (int i = 0; i < lst.Count; i++)
                        {
                            args[i] = invocation.Arguments[lst[i]];
                        }
                    }
                    var winObj = new UnwindObject(invocation.Arguments, StringTransfer);
                    var res = await finder.FindInCahceAsync(winObj);
                    if (res == null)
                    {
                        res = await proceed(invocation, proceedInfo);
                        unwinAccsstor.Invocation = invocation;
                        unwinAccsstor.ProceedInfo = proceedInfo;
                        unwinAccsstor.Proceed = proceed;
                        await finder.SetInCahceAsync(winObj, res);
                        if (res is AutoCacheResultBase result)
                        {
                            result.Args = args;
                            result.Status = AutoCacheStatus.MethodHit;
                        }
                        return res;
                    }
                    if (res is AutoCacheResultBase resultr)
                    {
                        resultr.Args = args;
                        resultr.Status = AutoCacheStatus.CacheHit;
                    }
                    invocation.ReturnValue = res;
                    return res;
                }
                else
                {
                    var res = await proceed(invocation, proceedInfo);
                    if (res is AutoCacheResultBase result)
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

    }
}
