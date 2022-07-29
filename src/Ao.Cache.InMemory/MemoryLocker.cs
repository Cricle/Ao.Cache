using System;

namespace Ao.Cache.InMemory
{
    internal struct MemoryLocker : ILocker
    {
        public string Resource { get; set; }

        public string LockId { get; set; }

        public bool IsAcquired { get; set; }

        public DateTime CreateTime { get; set; }

        public TimeSpan ExpireTime { get; set; }

        public bool IsInvalid => CreateTime.Add(ExpireTime) >= DateTime.Now;

        public MemoryLockFactory MemoryLockFactory { get; set; }

        public void Dispose()
        {
            MemoryLockFactory.Remove(Resource);
        }
    }
}
