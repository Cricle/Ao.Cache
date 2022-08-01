using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ao.Cache.InMemory
{
    public class MemoryLockFactory : ILockerFactory,IDisposable
    {
        private readonly SemaphoreSlim slim = new SemaphoreSlim(1,1);

        private readonly Dictionary<string, MemoryLocker> resourceMap = new Dictionary<string, MemoryLocker>();

        public int TryDelayMS { get; set; } = 200;

        private MemoryLocker WriteMap(DateTime now, string resource, TimeSpan expiryTime)
        {
            var locker = new MemoryLocker
            {
                CreateTime = now,
                ExpireTime = expiryTime,
                Resource = resource,
                IsAcquired = true,
                LockId = Guid.NewGuid().ToString(),
                MemoryLockFactory = this
            };
            resourceMap[resource] = locker;
            return locker;
        }
        private MemoryLocker MakeFail(DateTime now, string resource, TimeSpan expiryTime)
        {
            return new MemoryLocker
            {
                CreateTime = now,
                ExpireTime = expiryTime,
                Resource = resource,
                IsAcquired = false,
                LockId = Guid.NewGuid().ToString(),
                MemoryLockFactory = this
            };
        }
        private MemoryLocker TryAndWrite(DateTime now, string resource, TimeSpan expiryTime)
        {
            if (resourceMap.TryGetValue(resource, out var lk) && !lk.IsInvalid)
            {
                return null;
            }
            return WriteMap(now, resource, expiryTime);
        }
        public ILocker CreateLock(string resource, TimeSpan expiryTime)
        {
            ILocker locker = null;
            var now = DateTime.Now;
            var endTime = now.Add(expiryTime);
            while ((now = DateTime.Now) <= endTime)
            {
                slim.Wait();
                try
                {
                    if (!resourceMap.TryGetValue(resource, out var lk) || !lk.IsInvalid)
                    {
                        locker = TryAndWrite(now, resource, expiryTime);
                        if (locker != null)
                        {
                            break;
                        }

                    }
                }
                finally
                {
                    slim.Release();
                }
                Thread.Sleep(TryDelayMS + ThreadSafeRandom.Next(-100, 100));
            }
            if (locker == null)
            {
                locker = MakeFail(now, resource, expiryTime);
            }
            return locker;
        }

        public async Task<ILocker> CreateLockAsync(string resource, TimeSpan expiryTime)
        {
            ILocker locker = null;
            var now = DateTime.Now;
            var endTime = now.Add(expiryTime);
            while ((now = DateTime.Now) <= endTime)
            {
                await slim.WaitAsync();
                try
                {
                    if (!resourceMap.TryGetValue(resource, out var lk) || !lk.IsInvalid)
                    {
                        locker = TryAndWrite(now, resource, expiryTime);
                        if (locker != null)
                        {
                            break;
                        }

                    }
                }
                finally
                {
                    slim.Release();
                }
                await Task.Delay(TryDelayMS + ThreadSafeRandom.Next(-100, 100));
            }
            if (locker == null)
            {
                locker = MakeFail(now, resource, expiryTime);
            }
            return locker;
        }
        internal void Remove(string resource)
        {
            slim.Wait();
            try
            {
                resourceMap.Remove(resource);
            }
            finally
            {
                slim.Release();
            }
        }

        public void Dispose()
        {
            slim.Dispose();
        }
    }
}
