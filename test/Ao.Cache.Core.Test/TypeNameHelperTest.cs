using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    class A
    {

    }
    class B<T>
    {

    }
    class C<T>
    {

    }
    [TestClass]
    public class TypeNameHelperTest
    {
        [TestMethod]
        public void GenericTypes()
        {
            var t = typeof(B<C<int>>);
            var act = TypeNameHelper.GetFriendlyFullName(t);
            for (int i = 0; i < 2; i++)
            {
                Assert.AreEqual("Ao.Cache.Core.Test.B<C<Int32>>", act);
            }
        }
        [TestMethod]
        public void NoGenericTypes()
        {
            var t = typeof(A);
            var act = TypeNameHelper.GetFriendlyFullName(t);
            for (int i = 0; i < 2; i++)
            {
                Assert.AreEqual("Ao.Cache.Core.Test.A", act);
            }
        }
    }
}
