using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    [TestClass]
    public class DelegateDataAccesstorTest
    {
        [TestMethod]
        public async Task InvokeWithDelegate()
        {
            var inst = new object();
            var res = await new DelegateDataAccesstor<int,object>(_ => Task.FromResult(inst)).FindAsync(0);
            Assert.AreEqual(inst, res);
        }
        [TestMethod]
        public async Task InvokeWithBatchDelegate()
        {
            var inst = new Dictionary<int,object>();
            var res = await new DelegateBatchDataAccesstor<int, object>(_ => Task.FromResult<IDictionary<int,object>>(inst)).FindAsync(null);
            Assert.AreEqual(inst, res);
        }
    }
}
