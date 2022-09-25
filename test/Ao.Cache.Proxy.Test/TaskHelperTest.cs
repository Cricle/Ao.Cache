using Ao.Cache.Proxy.Annotations;

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
