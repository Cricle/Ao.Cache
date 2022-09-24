using Ao.Cache.Proxy.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class TaskHelperTest
    {
        [TestMethod]
        public void MustComplated()
        {
            Assert.IsTrue(TaskHelper.ComplatedTask.IsCompleted);
        }
    }
}
