using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{

    [TestClass]
    public class DataFinderBaseTest
    {
        [TestMethod]
        public async Task FindInCache_WhenExists_ReturnCache()
        {
            var finder = new NullDataFinder();
            var str = await finder.FindInCahceAsync(1);
            Assert.AreEqual("1", str);
        }
        [TestMethod]
        public async Task FindInCache_WhenNotExists_ReturnCache()
        {
            var finder = new NullDataFinder();
            var str = await finder.FindInCahceAsync(99);
            Assert.IsNull(str);
        }
    }
}
