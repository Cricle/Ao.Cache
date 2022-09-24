namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class ActionTypeHelperTest
    {
        [TestMethod]
        public void NoAutoCacheResult_ReturnSameType()
        {
            for (int i = 0; i < 2; i++)
            {
                var t = ActionTypeHelper.GetActionType(typeof(object));

                Assert.AreEqual(typeof(object), t.ActualType);
                Assert.IsTrue(t.TypesEquals);
                Assert.AreEqual(typeof(IDataFinder<UnwindObject, object>), t.FinderType);
            }
        }
        [TestMethod]
        public void AutoCacheResult_ReturnNotSameType()
        {
            for (int i = 0; i < 2; i++)
            {
                var t = ActionTypeHelper.GetActionType(typeof(AutoCacheResult<object>));

                Assert.AreEqual(typeof(object), t.ActualType);
                Assert.IsFalse(t.TypesEquals);
                Assert.AreEqual(typeof(IDataFinder<UnwindObject, object>), t.FinderType);
            }
        }
    }
}
