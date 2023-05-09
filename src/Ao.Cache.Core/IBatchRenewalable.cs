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
    public interface ISyncBatchRenewalable<TIdentity>
    {
        long Renewal(IReadOnlyList<TIdentity> input);

        long Renewal(IDictionary<TIdentity, TimeSpan?> input);
    }
}
