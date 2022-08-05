using Ao.Cache.CastleProxy.Annotations;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public class LockInterceptor: NamedInterceptor
    {
        protected enum RunLockResultTypes
        {
            SkipNoLocker,
            InLocker,
        }
        protected readonly struct RunLockResult : IDisposable
        {
            public readonly ILocker Locker;

            public readonly RunLockResultTypes Type;

            public RunLockResult(RunLockResultTypes type)
            {
                Locker = null;
                Type = type;
            }
            public RunLockResult(ILocker locker, RunLockResultTypes type)
            {
                Locker = locker;
                Type = type;
            }

            public void Dispose()
            {
                Locker?.Dispose();
            }
        }
        private static readonly object locker = new object();
        private static readonly Dictionary<NamedInterceptorKey, NamedInterceptorValue> argCacheMap = new Dictionary<NamedInterceptorKey, NamedInterceptorValue>();

        public LockInterceptor(ILockerFactory lockerFactory)
        {
            LockerFactory = lockerFactory ?? throw new ArgumentNullException(nameof(lockerFactory));
        }

        public ILockerFactory LockerFactory { get; }

        protected override Dictionary<NamedInterceptorKey, NamedInterceptorValue> CacheMap => argCacheMap;

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            var locker = await RunLockAsync(invocation, proceedInfo);
            await OnGotLockResultAsync(invocation, proceedInfo, locker);
            if (locker.Type == RunLockResultTypes.InLocker)
            {
                using (locker.Locker)
                {
                    await proceed(invocation, proceedInfo);
                }
            }
            else
            {
                await proceed(invocation, proceedInfo);
            }
        }
        protected virtual Task OnGotLockResultAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo,RunLockResult result)
        {
#if NET452
            return Task.FromResult(false);
#else
            return Task.CompletedTask;
#endif
        }
        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var locker = await RunLockAsync(invocation, proceedInfo);
            await OnGotLockResultAsync(invocation, proceedInfo, locker);
            if (locker.Type == RunLockResultTypes.InLocker)
            {
                using (locker.Locker)
                {
                    return await proceed(invocation, proceedInfo);
                }
            }
            else
            {
                return await proceed(invocation, proceedInfo);
            }
        }
        private async Task<RunLockResult> RunLockAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            var attr = GetLockAttr(invocation);
            if (attr == null)
            {
                return new RunLockResult(RunLockResultTypes.SkipNoLocker);
            }
            var key = new NamedInterceptorKey(invocation.TargetType, invocation.Method);
            var lst = GetArgIndexs(key);
            var args = MakeArgs(lst, invocation.Arguments);
            var lockKey = GenerateKey(lst.Header, args);
            var locker = await LockerFactory.CreateLockAsync(lockKey, attr.ExpireTime);
            return new RunLockResult(locker, RunLockResultTypes.InLocker);
        }
        protected virtual string GenerateKey(string header, object[] args)
        {
            return KeyGenerator.Concat(header, args);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AutoLockAttribute GetLockAttr(IInvocation invocation)
        {
            return invocation.TargetType.GetCustomAttribute<AutoLockAttribute>() ?? 
                invocation.Method.GetCustomAttribute<AutoLockAttribute>();
        }
        protected override bool ParamterCanUse(ParameterInfo param)
        {
            return param.GetCustomAttribute<AutoLockSkipPartAttribute>() == null;
        }

        protected override object GetLocker()
        {
            return locker;
        }

    }
}
