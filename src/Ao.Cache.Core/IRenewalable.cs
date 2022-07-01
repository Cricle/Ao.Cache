using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IRenewalable<TIdentity>
    {
        Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time);
    }

}
