using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Exceptions;
using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public class LockInterceptor : NamedInterceptor
    {
        public LockInterceptor(ILockerFactory lockerFactory, ICacheNamedHelper namedHelper)
        {
            LockerFactory = lockerFactory ?? throw new ArgumentNullException(nameof(lockerFactory));
            NamedHelper = namedHelper;
        }

        public ILockerFactory LockerFactory { get; }

        public ICacheNamedHelper NamedHelper { get; }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            var locker = await LockHelper.GetLockAsync(invocation, LockerFactory,NamedHelper);
            await OnGotLockResultAsync(invocation, proceedInfo, locker);
            if (locker.Locker!=null)
            {
                if (locker.Type == RunLockResultTypes.InLocker&&
                    locker.Locker.IsAcquired)
                {
                    using (locker.Locker)
                    {
                        await proceed(invocation, proceedInfo);
                    }
                }
                else
                {
                    await GetLockFailAsync(invocation, proceedInfo, proceed, locker);
                }
            }
            else
            {
                await proceed(invocation, proceedInfo);
            }
        }
        protected virtual Task GetLockFailAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed,RunLockResult result)
        {
            throw new GetLockFailException { locker = result.Locker };
        }
        protected virtual Task<TResult> GetLockFailAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed, RunLockResult result)
        {
            throw new GetLockFailException { locker = result.Locker };
        }
        protected virtual Task OnGotLockResultAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, RunLockResult result)
        {
            return TaskHelper.ComplatedTask;
        }
        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var locker = await LockHelper.GetLockAsync(invocation, LockerFactory,NamedHelper);
            await OnGotLockResultAsync(invocation, proceedInfo, locker);
            if (locker.Locker != null)
            {
                if (locker.Type == RunLockResultTypes.InLocker &&
                    locker.Locker.IsAcquired)
                {
                    using (locker.Locker)
                    {
                       return await proceed(invocation, proceedInfo);
                    }
                }
                else
                {
                    return await GetLockFailAsync(invocation, proceedInfo, proceed, locker);
                }
            }
            else
            {
                return await proceed(invocation, proceedInfo);
            }
        }
    }
    public static class LockHelper
    {
        public static Task<RunLockResult> GetLockAsync(IInvocation invocation,
            ILockerFactory lockerFactory,
            ICacheNamedHelper namedHelper)
        {
            var attr = AutoLockAttrCache.Get(new NamedInterceptorKey(invocation.TargetType, invocation.Method));
            if (attr == null)
            {
                return Task.FromResult(new RunLockResult(RunLockResultTypes.SkipNoLocker));
            }
            return GetLockAsync(invocation, lockerFactory, namedHelper, attr.ExpireTime);
        }
        public static async Task<RunLockResult> GetLockAsync(IInvocation invocation,
            ILockerFactory lockerFactory,
            ICacheNamedHelper namedHelper,
            TimeSpan expireTime)
        {
            var key = new NamedInterceptorKey(invocation.TargetType, invocation.Method);
            var lst = namedHelper.GetArgIndexs(key);
            var args = namedHelper.MakeArgs(lst, invocation.Arguments);
            var lockKey = KeyGenerator.Concat(lst.Header, args);
            var locker = await lockerFactory.CreateLockAsync(lockKey, expireTime);
            return new RunLockResult(locker, RunLockResultTypes.InLocker);
        }
    }
}
