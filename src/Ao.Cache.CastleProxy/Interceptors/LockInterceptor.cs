using Ao.Cache.Proxy;
using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Exceptions;
using Ao.Cache.Proxy.Interceptors;
using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Interceptors
{
    public class LockInterceptor : AsyncInterceptorBase, IInterceptor
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
            var locker = await LockHelper.GetLockAsync(new InvocationInfo(invocation), LockerFactory, NamedHelper);
            await OnGotLockResultAsync(invocation, proceedInfo, locker);
            if (locker.Locker != null)
            {
                if (locker.Type == RunLockResultTypes.InLocker &&
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
        protected virtual Task GetLockFailAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed, RunLockResult result)
        {
            throw new GetLockFailException { Locker = result.Locker };
        }
        protected virtual Task<TResult> GetLockFailAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed, RunLockResult result)
        {
            throw new GetLockFailException { Locker = result.Locker };
        }
        protected virtual Task OnGotLockResultAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, RunLockResult result)
        {
            return TaskHelper.ComplatedTask;
        }
        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var locker = await LockHelper.GetLockAsync(new InvocationInfo(invocation), LockerFactory, NamedHelper);
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

        public void Intercept(IInvocation invocation)
        {
            InterceptSynchronous(invocation);
        }
    }
}
