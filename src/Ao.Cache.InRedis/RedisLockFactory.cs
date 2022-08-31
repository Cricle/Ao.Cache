using RedLockNet;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public class RedisLockFactory : ILockerFactory
    {
        public RedisLockFactory(IDistributedLockFactory lockFactory)
        {
            LockFactory = lockFactory ?? throw new ArgumentNullException(nameof(lockFactory));
        }

        public IDistributedLockFactory LockFactory { get; }

        public TimeSpan WaitTime { get; set; } = TimeSpan.FromSeconds(10);

        public TimeSpan RetryTime { get; set; } = TimeSpan.FromSeconds(10);

        public ILocker CreateLock(string resource, TimeSpan expiryTime)
        {
            var locker = LockFactory.CreateLock(resource, expiryTime, WaitTime, RetryTime);
            return new RedisLocker(locker, DateTime.Now, expiryTime);
        }

        public async Task<ILocker> CreateLockAsync(string resource, TimeSpan expiryTime)
        {
            var locker = await LockFactory.CreateLockAsync(resource, expiryTime, WaitTime, RetryTime);
            return new RedisLocker(locker, DateTime.Now, expiryTime);
        }
    }
}
