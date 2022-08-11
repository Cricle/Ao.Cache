using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    [TestClass]
    public class OptionalDataFinderTest
    {
        [TestMethod]
        public void DefaultIsDefaultDataFinderOptions()
        {
            var finder = new OptionalDataFinder<int,int>();
            Assert.IsInstanceOfType(finder.Options, typeof(DefaultDataFinderOptions<int, int>));
        }
        [TestMethod]
        public void SetNull_WasSetDefaultDataFinderOptions()
        {
            var finder = new OptionalDataFinder<int, int>();
            finder.Options = null;
            Assert.IsInstanceOfType(finder.Options, typeof(DefaultDataFinderOptions<int, int>));
        }

        [TestMethod]
        public void Set_WasEqualsSet()
        {
            var finder = new OptionalDataFinder<int, int>();
            var opt = new NullDataFinderOptions<int, int>();
            finder.Options = opt;
            Assert.Equals(finder.Options, opt);
        }
    }
}
