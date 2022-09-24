using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test.Model
{
    [TestClass]
    public class AutoCacheResultRawFetcherTest
    {
        [TestMethod]
        public void GetRawResult_ValueType()
        {
            GetRawResult(123);
        }
        [TestMethod]
        public void SetRawResult_ValueType()
        {
            SetRawResult(123);
        }
        [TestMethod]
        public void GetRawResult_Class()
        {
            GetRawResult(new object());
        }
        [TestMethod]
        public void SetRawResult_Class()
        {
            SetRawResult(123);
        }
        private void GetRawResult<T>(T input)
        {
            for (int i = 0; i < 2; i++)
            {
                var inst = new AutoCacheResult<T> { RawData = input };
                var val = AutoCacheResultRawFetcher.GetRawResult(inst, typeof(T));
                Assert.AreEqual(input, val);
            }
        }
        private void SetRawResult<T>(T input)
        {
            for (int i = 0; i < 2; i++)
            {
                var inst = new AutoCacheResult<int> { RawData = 0 };
                AutoCacheResultRawFetcher.SetRawResult(inst, input, typeof(int));
                Assert.AreEqual(input, inst.RawData);
            }
        }
    }
}
