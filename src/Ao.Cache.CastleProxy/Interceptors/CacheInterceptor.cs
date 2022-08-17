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
        class ActualTypeInfos
        {
            public Type ActualType;

            public Type FinderType;

            public bool TypesEquals;

            public Func<CacheInterceptor, IInvocation, IInvocationProceedInfo, object, object> Method;
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
                        t = new ActualTypeInfos { ActualType = type, TypesEquals=true };
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(AutoCacheResult<>))
                        {
                            t.ActualType = type.GenericTypeArguments[0];
                            t.Method = CompileMethod(type, t.ActualType);
                            t.TypesEquals = false;
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
                var res = await proceed(invocation, proceedInfo).ConfigureAwait(false);
                if (res is AutoCacheResult<TOut> o)
                {
                    return o.RawData;
                }
                return (TOut)(object)res;
            };
        }
        class DecoratorHelper
        {
            private static readonly Dictionary<NamedInterceptorKey, AutoCacheDecoratorBaseAttribute[]> m = new Dictionary<NamedInterceptorKey, AutoCacheDecoratorBaseAttribute[]>();
            private static readonly object locker = new object();

            public static AutoCacheDecoratorBaseAttribute[] Get(in NamedInterceptorKey info)
            {
                if (!m.TryGetValue(info,out var attr))
                {
                    lock (locker)
                    {
                        if (!m.TryGetValue(info, out attr))
                        {
                            var attrs = new List<AutoCacheDecoratorBaseAttribute>();
                            var typeAttr = info.TargetType.GetCustomAttributes<AutoCacheDecoratorBaseAttribute>();
                            if (typeAttr!=null)
                            {
                                attrs.AddRange(typeAttr);
                            }
                            var methodAttr=info.Method.GetCustomAttributes<AutoCacheDecoratorBaseAttribute>();
                            if (methodAttr!=null)
                            {
                                attrs.AddRange(methodAttr);
                            }
                            attrs.Sort((a, b) => a.Order - b.Order);
                            attr = attrs.ToArray();
                            m[info] = attr;
                        }
                    }
                }
                return attr;
            }
        }
        protected async Task<AutoCacheResult<TResult>> CoreInterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<Task<TResult>> proceed)
        {
            var rr = new AutoCacheResult<TResult>();
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var finderFactory = scope.ServiceProvider.GetRequiredService<IDataFinderFactory>();
                var finder = finderFactory.Create(new CastleDataAccesstor<UnwindObject, TResult> (proceed));
                var key = new NamedInterceptorKey(invocation.TargetType, invocation.Method);
                var attr = DecoratorHelper.Get(key);

                var winObj = NamedHelper.GetUnwindObject(key, invocation.Arguments);
                var ctx = new AutoCacheDecoratorContext<TResult>(
                    invocation, proceedInfo, scope.ServiceProvider, finder, winObj);
                for (int i = 0; i < attr.Length; i++)
                {
                    await attr[i].DecorateAsync(ctx).ConfigureAwait(false);
                }
                var res = await finder.FindInCacheAsync(winObj).ConfigureAwait(false);
                if (res == null)
                {
                    res = await proceed().ConfigureAwait(false);
                    await finder.SetInCacheAsync(winObj, res).ConfigureAwait(false);
                    rr.RawData = res;
                    rr.Status = AutoCacheStatus.MethodHit;
                    for (int i = 0; i < attr.Length; i++)
                    {
                        await attr[i].FoundInMethodAsync(ctx,res).ConfigureAwait(false);
                    }
                    return rr;
                }
                
                rr.Status = AutoCacheStatus.CacheHit;
                rr.RawData = res;
                invocation.ReturnValue = res;
                for (int i = 0; i < attr.Length; i++)
                {
                    await attr[i].FoundInCacheAsync(ctx, res).ConfigureAwait(false);
                }
                return rr;
            }
        }
        private static readonly object hasAutoCacheLocker = new object();
        private static readonly Dictionary<Type, bool> hasAutoCache = new Dictionary<Type, bool>();
        private static bool HasAutoCache(Type tuple, IInvocation invocation)
        {
            if (!hasAutoCache.TryGetValue(tuple, out var b))
            {
                lock (hasAutoCacheLocker)
                {
                    if (!hasAutoCache.TryGetValue(tuple, out b))
                    {
                        b = (invocation.TargetType.GetCustomAttribute<AutoCacheAttribute>() ??
                            invocation.Method.GetCustomAttribute<AutoCacheAttribute>()) != null;
                        hasAutoCache[tuple] = b;
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
                var res = await proceed(invocation, proceedInfo).ConfigureAwait(false);
                if (res is IAutoCacheResult result)
                {
                    result.Status = AutoCacheStatus.Skip;
                }
                return res;
            }
            var key = new NamedInterceptorKey(invocation.TargetType, invocation.Method);
            var attr = DecoratorHelper.Get(key);
            var ctx = new AutoCacheInvokeDecoratorContext<TResult>(invocation, proceedInfo, ServiceScopeFactory, proceed);
            for (int i = 0; i < attr.Length; i++)
            {
                await attr[i].InterceptBeginAsync(ctx);
            }
            try
            {
                var actualTypeInfo = GetActionType(originType);
                if (actualTypeInfo.TypesEquals)
                {
                    var res = await CoreInterceptAsync(invocation, proceedInfo, () => proceed(invocation, proceedInfo)).ConfigureAwait(false);
                    var result = new AutoCacheInvokeResultContext<TResult>(res.RawData, res, null);
                    for (int i = 0; i < attr.Length; i++)
                    {
                        await attr[i].InterceptEndAsync(ctx, result);
                    }
                    return res.RawData;
                }
                var rr = await (Task<TResult>)actualTypeInfo.Method(this, invocation, proceedInfo, proceed);
                invocation.ReturnValue = rr;
                var cacheResult = new AutoCacheInvokeResultContext<TResult>(rr, rr as IAutoCacheResult, null);
                for (int i = 0; i < attr.Length; i++)
                {
                    await attr[i].InterceptEndAsync(ctx, cacheResult);
                }
                return rr;
            }
            catch (Exception ex)
            {
                for (int i = 0; i < attr.Length; i++)
                {
                    await attr[i].InterceptExceptionAsync(ctx, ex);
                }
                throw;
            }
            finally
            {
                for (int i = 0; i < attr.Length; i++)
                {
                    await attr[i].InterceptFinallyAsync(ctx);
                }
            }
        }
    }
}
