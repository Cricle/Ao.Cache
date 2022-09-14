using System;

namespace Ao.Cache.Proxy.Interceptors
{
    public readonly struct RunLockResult : IDisposable
    {
        public readonly ILocker Locker;

        public readonly RunLockResultTypes Type;

        internal RunLockResult(RunLockResultTypes type)
        {
            Locker = null;
            Type = type;
        }
        internal RunLockResult(ILocker locker, RunLockResultTypes type)
        {
            Locker = locker;
            Type = type;
        }

        public bool IsLocked => Type == RunLockResultTypes.InLocker && Locker != null && Locker.IsAcquired;

        public void Dispose()
        {
            Locker?.Dispose();
        }
    }
}
