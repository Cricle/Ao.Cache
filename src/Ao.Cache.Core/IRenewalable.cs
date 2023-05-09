using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IRenewalable<TIdentity>
    {
        Task<bool> RenewalAsync(TIdentity identity);

        Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time);
    }
    public interface ISyncRenewalable<TIdentity>
    {
        bool Renewal(TIdentity identity);

        bool Renewal(TIdentity identity, TimeSpan? time);
    }
}
