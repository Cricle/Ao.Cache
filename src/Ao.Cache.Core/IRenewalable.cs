using System;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IRenewalable
    {
        Task<bool> RenewalAsync(object identity);

        Task<bool> RenewalAsync(object identity, TimeSpan? time);
    }
    public interface IRenewalable<TIdentity>
    {
        Task<bool> RenewalAsync(TIdentity identity);

        Task<bool> RenewalAsync(TIdentity identity, TimeSpan? time);
    }

}
