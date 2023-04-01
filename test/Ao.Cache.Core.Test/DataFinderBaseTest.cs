using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var str = await finder.FindInCacheAsync(1);
            Assert.AreEqual("1", str);
        }
        [TestMethod]
        public async Task FindInCache_WhenNotExists_ReturnCache()
        {
            var finder = new NullDataFinder();
            var str = await finder.FindInCacheAsync(99);
            Assert.IsNull(str);
        }
        [TestMethod]
        public async Task FindInDb_WithSetCache()
        {
            var finder = new NullDataFinder();
            var str = await finder.FindInDbAsync(new DelegateDataAccesstor<int,string>(x=>Task.FromResult("9")),99);
            Assert.AreEqual("9", str);
        }
        [TestMethod]
        public async Task SetInCache()
        {
            var finder = new NullDataFinder();
            var ok = await finder.SetInCacheAsync(1, "1");
            Assert.IsTrue(ok);
        }
    }
}
