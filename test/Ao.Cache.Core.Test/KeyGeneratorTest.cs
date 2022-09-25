using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Ao.Cache.Core.Test
{
    [TestClass]
    public class KeyGeneratorTest
    {
        [TestMethod]
        public void Concat_A_BS()
        {
            var act = KeyGenerator.Concat("a", new object[] { "b", "c" });
            Assert.AreEqual("a_b_c", act);
            act = KeyGenerator.Concat("a", new object[] { });
            Assert.AreEqual("a", act);
        }
        [TestMethod]
        public void Concat_A_B()
        {
            var act = KeyGenerator.Concat("a", "b");
            Assert.AreEqual("a_b", act);
            act = KeyGenerator.Concat("a", (string)null);
            Assert.AreEqual($"a_{KeyGenerator.NullString}", act);
        }
        [TestMethod]
        public void Concat_A_B_C()
        {
            var act = KeyGenerator.Concat("a", "b", "c");
            Assert.AreEqual("a_b_c", act);
            act = KeyGenerator.Concat("a", null, null);
            Assert.AreEqual($"a_{KeyGenerator.NullString}_{KeyGenerator.NullString}", act);
        }
        [TestMethod]
        public void Concat_A_B_C_D()
        {
            var act = KeyGenerator.Concat("a", "b", "c", "d");
            Assert.AreEqual("a_b_c_d", act);
            act = KeyGenerator.Concat("a", null, null, null);
            Assert.AreEqual($"a_{KeyGenerator.NullString}_{KeyGenerator.NullString}_{KeyGenerator.NullString}", act);
        }
        [TestMethod]
        public void Concat_A_B_C_D_E()
        {
            var act = KeyGenerator.Concat("a", "b", "c", "d", "e");
            Assert.AreEqual("a_b_c_d_e", act);
            act = KeyGenerator.Concat("a", null, null, null, null);
            Assert.AreEqual($"a_{KeyGenerator.NullString}_{KeyGenerator.NullString}_{KeyGenerator.NullString}_{KeyGenerator.NullString}", act);
        }
        [TestMethod]
        public void Concat_WithNull()
        {
            var act = KeyGenerator.Concat("a", (string)null);
            Assert.AreEqual($"a_{KeyGenerator.NullString}", act);
        }
        [TestMethod]
        public void GivenNullCall_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => KeyGenerator.Concat("a", null));
        }
    }
}
