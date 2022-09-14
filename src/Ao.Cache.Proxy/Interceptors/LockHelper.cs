using System;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Interceptors
{
    public static class LockHelper
    {
        public static Task<RunLockResult> GetLockAsync(IInvocationInfo invocation,
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
        public static async Task<RunLockResult> GetLockAsync(IInvocationInfo invocation,
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
