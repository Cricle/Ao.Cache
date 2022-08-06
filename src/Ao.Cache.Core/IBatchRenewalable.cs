using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchRenewalable<TIdentity>
    {
        Task<long> RenewalAsync(IReadOnlyList<TIdentity> input);

        Task<long> RenewalAsync(IDictionary<TIdentity, TimeSpan?> input);
    }
    public interface IBatchRenewalable
    {
        Task<long> RenewalAsync(IReadOnlyList<object> input);

        Task<long> RenewalAsync(IDictionary<object, TimeSpan?> input);
    }
}
