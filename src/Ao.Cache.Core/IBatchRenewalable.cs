using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IBatchRenewalable<TIdentity>
    {
        Task<IDictionary<TIdentity,bool>> RenewalAsync(IReadOnlyDictionary<TIdentity,TimeSpan?> input);
    }

}
