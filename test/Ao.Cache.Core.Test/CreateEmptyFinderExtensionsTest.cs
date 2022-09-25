using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    [TestClass]
    public class CreateEmptyFinderExtensionsTest
    {
        class NullDataFinderFactory : IDataFinderFactory,IBatchDataFinderFactory
        {
            public IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IDataAccesstor<TIdentity, TEntity> accesstor)
            {
                return null;
            }

            public IBatchDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IBatchDataAccesstor<TIdentity, TEntity> accesstor)
            {
                return null;
            }
        }
        [TestMethod]
        public void Empty()
        {
            var fc = new NullDataFinderFactory();
            CreateEmptyFinderExtensions.CreateEmpty<object, object>((IDataFinderFactory)fc);
            CreateEmptyFinderExtensions.CreateEmpty<object, object>((IBatchDataFinderFactory)fc);
        }
    }
}
