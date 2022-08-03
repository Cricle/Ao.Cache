using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Model;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        class ActualTypeInfos
        {
            public Type ActualType { get; set; }

            public Type FinderType { get; set; }

            public Func<CacheInterceptor, IInvocation, IInvocationProceedInfo, object, object> Method { get; set; }
        }
        private static readonly object actionTypeLocker = new object();
        private static readonly Dictionary<Type, ActualTypeInfos> actualTypes = new Dictionary<Type, ActualTypeInfos>();
        private static ActualTypeInfos GetActionType(Type type)
        {
            if (!actualTypes.TryGetValue(type, out var t))
            {
                lock (actionTypeLocker)
                {
                    if (!actualTypes.TryGetValue(type, out t))
                    {
                        t = new ActualTypeInfos { ActualType = type };
                        if (type.GetGenericTypeDefinition() == typeof(AutoCacheResult<>))
                        {
                            t.ActualType = type.GenericTypeArguments[0];
                            t.Method = CompileMethod(type, t.ActualType);
                        }
                        t.FinderType = typeof(IDataFinder<,>).MakeGenericType(typeof(UnwindObject), t.ActualType);
                        actualTypes[type] = t;
                    }
                }
            }
            return t;
        }
        private static Func<CacheInterceptor, IInvocation, IInvocationProceedInfo, object, object> CompileMethod(Type prevType, Type actualType)
        {
            var coreInterceptMethod = typeof(CacheInterceptor).GetMethod(nameof(CoreInterceptAsync), BindingFlags.Instance | BindingFlags.NonPublic);
            var method = coreInterceptMethod.MakeGenericMethod(actualType);
            var par0 = Expression.Parameter(typeof(CacheInterceptor));
            var par1 = Expression.Parameter(typeof(IInvocation));
            var par2 = Expression.Parameter(typeof(IInvocationProceedInfo));
            var par3 = Expression.Parameter(typeof(object));

            var par3Convert = Expression.Convert(par3,
                typeof(Func<,,>).MakeGenericType(typeof(IInvocation), typeof(IInvocationProceedInfo), typeof(Task<>).MakeGenericType(prevType)));

            var caseMethod = typeof(CacheInterceptor).GetMethod(nameof(Case), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(prevType, actualType);


            var par3Case = Expression.Call(null, caseMethod, par1, par2, par3Convert);

            var body = Expression.Call(par0, method, par1, par2, par3Case);
            return Expression.Lambda<Func<CacheInterceptor, IInvocation, IInvocationProceedInfo, object, object>>(body,
                par0, par1, par2, par3).Compile();
        }
        private static Func<Task<TOut>> Case<TResult, TOut>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            return async () =>
            {
                var res = await proceed(invocation, proceedInfo);
                if (res is AutoCacheResult<TOut> o)
                {
                    return o.RawData;
                }
                return (TOut)(object)res;
            };
        }
        protected async Task<AutoCacheResult<TResult>> CoreInterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<Task<TResult>> proceed)
        {
            var rr = new AutoCacheResult<TResult>();
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var finder = scope.ServiceProvider.GetRequiredService<IDataFinder<UnwindObject, TResult>>();
                if (finder is IWithDataAccesstorFinder<UnwindObject, TResult> f &&
                    f.DataAccesstor is CastleDataAccesstor<UnwindObject, TResult> unwinAccsstor)
                {
                    var key = new NamedInterceptorKey(invocation.TargetType, invocation.Method);
                    var lst = GetArgIndexs(key, invocation.Method);
                    var args = MakeArgsWithHeader(lst, invocation.Arguments);
                    var winObj = new UnwindObject(args, StringTransfer);
                    var res = await finder.FindInCahceAsync(winObj);
                    if (res == null)
                    {
                        res = await proceed();
                        unwinAccsstor.Proceed = proceed;
                        await finder.SetInCahceAsync(winObj, res);
                        rr.RawData = res;
                        rr.Status = AutoCacheStatus.MethodHit;
                        return rr;
                    }
                    rr.Status = AutoCacheStatus.CacheHit;
                    rr.RawData = res;
                    invocation.ReturnValue = res;
                    return rr;
                }
                else
                {
                    var res = await proceed();
                    rr.RawData = res;
                    rr.Status = AutoCacheStatus.NotSupportFinderOrAccesstor;
                    return rr;
                }
            }
        }
        private static readonly object hasAutoCacheLocker = new object();
        private static readonly Dictionary<Type, bool> hasAutoCache = new Dictionary<Type, bool>();
        private static bool HasAutoCache(Type tuple, IInvocation invocation)
        {
            if (!hasAutoCache.TryGetValue(tuple,out var b))
            {
                lock (hasAutoCacheLocker)
                {
                    if (!hasAutoCache.TryGetValue(tuple, out b))
                    {
                        b = (invocation.TargetType.GetCustomAttribute<AutoCacheAttribute>() ??
                            invocation.Method.GetCustomAttribute<AutoCacheAttribute>()) != null;

                    }
                }
            }
            return b;
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var originType = typeof(TResult);
            if (!HasAutoCache(originType, invocation))
            {
                var res = await proceed(invocation, proceedInfo);
                if (res is IAutoCacheResult result)
                {
                    result.Status = AutoCacheStatus.Skip;
                }
                return res;
            }
            var actualTypeInfo = GetActionType(typeof(TResult));
            if (originType == actualTypeInfo.ActualType)
            {
                var res= await CoreInterceptAsync(invocation, proceedInfo, () => proceed(invocation, proceedInfo));
                return res.RawData;
            }
            dynamic rr = actualTypeInfo.Method(this, invocation, proceedInfo, proceed);
            await rr;
            var r = rr.Result;
            invocation.ReturnValue = r;
            return r;
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
