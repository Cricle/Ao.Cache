using RedLockNet;
using System;

namespace Ao.Cache.InRedis
{
    internal readonly struct RedisLocker : ILocker
    {
        public RedisLocker(IRedLock locker, in DateTime createTime, in TimeSpan expireTime)
        {
            Locker = locker;
            CreateTime = createTime;
            ExpireTime = expireTime;
        }

        public IRedLock Locker { get; }

        public string Resource => Locker.Resource;

        public string LockId => Locker.LockId;

        public bool IsAcquired => Locker.IsAcquired;

        public DateTime CreateTime { get; }

        public TimeSpan ExpireTime { get; }

        public void Dispose()
        {
            Locker.Dispose();
        }
    }
}
