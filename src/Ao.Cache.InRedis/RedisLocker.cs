using RedLockNet;
using System;

namespace Ao.Cache.InRedis
{
    internal class RedisLocker : ILocker
    {
        public RedisLocker(IRedLock locker, DateTime createTime, TimeSpan expireTime)
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

        ~RedisLocker() 
        {
            Locker?.Dispose();
        }

        public void Dispose()
        {
            Locker.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
