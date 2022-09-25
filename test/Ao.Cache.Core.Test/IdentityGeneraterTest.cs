using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ao.Cache.Core.Test
{
    [TestClass]
    public class IdentityGeneraterTest
    {
        [TestMethod]
        public void Part_Must_Return_ToString()
        {
            var d = new IdentityGenerater<int, int>();

            Assert.AreEqual("1", d.GetPart(1));
        }
        [TestMethod]
        public void Head_Must_Return_FriendlyTypeName()
        {
            var d = new IdentityGenerater<int, int>();

            Assert.AreEqual(TypeNameHelper.GetFriendlyFullName(typeof(int)), d.GetHead());
        }
        [TestMethod]
        public void EntityKey_Must_Return_HeadWithPart()
        {
            var d = new IdentityGenerater<int, int>();

            var act = d.GetEntryKey(1);

            Assert.AreEqual(TypeNameHelper.GetFriendlyFullName(typeof(int)) + ".1", act);
        }
    }
}
