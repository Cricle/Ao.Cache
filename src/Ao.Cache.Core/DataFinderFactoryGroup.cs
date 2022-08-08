using System.Collections.Generic;
using System.Linq;

namespace Ao.Cache
{
    public class DataFinderFactoryGroup : List<IDataFinderFactory>, IDataFinderFactory
    {
        public IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new DataFinderGroup<TIdentity, TEntity>(this.Select(x => x.Create(accesstor)));
        }
    }
}
