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
        public void SyncInvokeWithDelegate()
        {
            var inst = new object();
            var res = new DelegateSyncDataAccesstor<int, object>(_ => inst).Find(0);
            Assert.AreEqual(inst, res);
        }
#if NET6_0||NETSTANDARD2_1
        [TestMethod]
        public void SyncInvokeWithDelegateValueTask()
        {
            var inst = new object();
            var res = new DelegateSyncDataAccesstor<int, object>(_ => new ValueTask<object>(inst)).Find(0);
            Assert.AreEqual(inst, res);
        }
#endif
        [TestMethod]
        public async Task InvokeWithDelegateSyncInput()
        {
            var inst = new object();
            var res = await new DelegateDataAccesstor<int, object>(_ => inst).FindAsync(0);
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
