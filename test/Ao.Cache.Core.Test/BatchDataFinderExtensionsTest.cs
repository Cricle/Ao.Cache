using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    [TestClass]
    public class BatchDataFinderExtensionsTest
    {
        class Batch : IBatchCacheFinder<int, string>
        {
            public Task<IDictionary<int, string>> FindInCahceAsync(IEnumerable<int> identity)
            {
                throw new NotImplementedException();
            }

            public Task<bool> SetInCahceAsync(IEnumerable<KeyValuePair<int, string>> pairs)
            {
                throw new NotImplementedException();
            }
        }
        //[TestMethod]
        //public void GivenNullCall_MustThrowException()
        //{
        //    var finder = new NullDataFinder();
        //    BatchDataFinderExtensions.FindAsync<int,string>(new Batch(), new[] {1});
        //    Assert.ThrowsException<ArgumentNullException>(() => BatchDataFinderExtensions.FindAsync<int, string>(null, 1));
        //    Assert.ThrowsException<ArgumentNullException>(() => BatchDataFinderExtensions.FindAsync<int, string>(null, new int[] { 1 }));
        //    Assert.ThrowsException<ArgumentNullException>(() => BatchDataFinderExtensions.FindAsync(new Batch(), (IEnumerable<int>)null));
        //}
    }
}
