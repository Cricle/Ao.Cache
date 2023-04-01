using System.Collections.Generic;
using System.Linq;

namespace Ao.Cache
{
    public class BatchDataFinderFactoryGroup : List<IBatchDataFinderFactory>, IBatchDataFinderFactory
    {
        public IBatchDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IBatchDataAccesstor<TIdentity, TEntity> accesstor)
        {
            return new BatchDataFinderGroup<TIdentity, TEntity>(this.Select(x => x.CreateBatch(accesstor)));
        }

        public IBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>()
        {
            throw new System.NotImplementedException();
        }

        public IWithBatchDataFinder<TIdentity, TEntity> CreateBatch<TIdentity, TEntity>(IBatchDataAccesstor<TIdentity, TEntity> accesstor)
        {
            throw new System.NotImplementedException();
        }
    }
}
