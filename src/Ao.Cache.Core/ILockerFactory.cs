using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface ILockerFactory
    {
        ILocker CreateLock(string resource, TimeSpan expiryTime);

        Task<ILocker> CreateLockAsync(string resource, TimeSpan expiryTime);
    }
}
