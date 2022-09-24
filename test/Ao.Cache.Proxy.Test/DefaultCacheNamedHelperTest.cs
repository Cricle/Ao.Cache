using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test
{
    [ExcludeFromCodeCoverage]
    class A
    {
        public void Go(int a, [KeySkipPart] string b)
        {

        }
        public void Go1(int a)
        {

        }
    }
    [ExcludeFromCodeCoverage]
    class NullStringTransfer : IStringTransfer
    {
        public string Combine(object header, params object[] args)
        {
            throw new NotImplementedException();
        }

        public string ToString(object data)
        {
            throw new NotImplementedException();
        }
    }
    [ExcludeFromCodeCoverage]
    class NullStringTransfer1 : IStringTransfer
    {
        public string Combine(object header, params object[] args)
        {
            throw new NotImplementedException();
        }

        public string ToString(object data)
        {
            throw new NotImplementedException();
        }
    }
    [StringTransfer(typeof(NullStringTransfer))]
    [ExcludeFromCodeCoverage]
    class B
    {
        public void NoTag()
        {

        }

        [StringTransfer(typeof(NullStringTransfer1))]
        public void Tag()
        {

        }
    }
    [TestClass]
    public class DefaultCacheNamedHelperTest
    {
        [TestMethod]
        public void GivenNullCall_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DefaultCacheNamedHelper(null));
        }
        [TestMethod]
        public void GetArgIndexs()
        {
            for (int i = 0; i < 2; i++)
            {
                var val = DefaultCacheNamedHelper.Default.GetArgIndexs(new NamedInterceptorKey(typeof(A), typeof(A).GetMethod("Go")));

                Assert.AreEqual(DefaultStringTransfer.Default, val.StringTransfer);
                Assert.AreEqual("Ao.Cache.Proxy.Test.A.Go", val.Header);
                Assert.AreEqual(0, val.ArgIndexs[0]);

                val = DefaultCacheNamedHelper.Default.GetArgIndexs(new NamedInterceptorKey(typeof(A), typeof(A).GetMethod("Go1")));

                Assert.AreEqual(DefaultStringTransfer.Default, val.StringTransfer);
                Assert.AreEqual("Ao.Cache.Proxy.Test.A.Go1", val.Header);
                Assert.IsNull(val.ArgIndexs);
            }
        }
        [TestMethod]
        public void GetEmptyArg()
        {
            var arr = new object[] {1};
            var val = DefaultCacheNamedHelper.Default.MakeArgs(
                new NamedInterceptorValue(null, DefaultStringTransfer.Default, "aaa"),
                arr);
            Assert.AreEqual(arr, val);
        }
        [TestMethod]
        public void GetNoEmptyArg()
        {
            var arr = new object[] { 1,2,3 };
            var val = DefaultCacheNamedHelper.Default.MakeArgs(
                new NamedInterceptorValue(new int[] {0}, DefaultStringTransfer.Default, "aaa"),
                arr);
            Assert.AreEqual(1, val.Length);
            Assert.AreEqual(arr[0], val[0]);
        }
        [TestMethod]
        public void GetEmptyArgWithHeader()
        {
            var arr = new object[] { 1 };
            var val = DefaultCacheNamedHelper.Default.MakeArgsWithHeader(
                new NamedInterceptorValue(null, DefaultStringTransfer.Default, "aaa"),
                arr);
            Assert.AreEqual(2, val.Length);
            Assert.AreEqual("aaa", val[0]);
            Assert.AreEqual(arr[0], val[1]);
        }
        [TestMethod]
        public void GetNoEmptyArgWithHeader()
        {
            var arr = new object[] { 1,2,3 };
            var val = DefaultCacheNamedHelper.Default.MakeArgsWithHeader(
                new NamedInterceptorValue(new int[] { 0 }, DefaultStringTransfer.Default, "aaa"),
                arr);
            Assert.AreEqual(2, val.Length);
            Assert.AreEqual("aaa", val[0]);
            Assert.AreEqual(arr[0], val[1]);
        }
        [TestMethod]
        public void GetUnwindObject_IgnoreIndex()
        {
            var arr = new object[] { 1, 2 };
            var val = DefaultCacheNamedHelper.Default.GetUnwindObject(
                new NamedInterceptorKey(typeof(A), typeof(A).GetMethod("Go")),
                arr,
                true);
            Assert.AreEqual(2, val.Objects.Length);
            Assert.AreEqual(1, val.Objects[0]);
            Assert.AreEqual(2, val.Objects[1]);
            Assert.AreEqual(DefaultStringTransfer.Default, val.ObjectTransfer);
            Assert.AreEqual("Ao.Cache.Proxy.Test.A.Go", val.Header);
        }
        [TestMethod]
        public void GetUnwindObject_NotIgnoreIndex()
        {
            var arr = new object[] { 1, 2 };
            var val = DefaultCacheNamedHelper.Default.GetUnwindObject(
                new NamedInterceptorKey(typeof(A), typeof(A).GetMethod("Go")),
                arr);
            Assert.AreEqual(1, val.Objects.Length);
            Assert.AreEqual(1, val.Objects[0]);
            Assert.AreEqual(DefaultStringTransfer.Default, val.ObjectTransfer);
            Assert.AreEqual("Ao.Cache.Proxy.Test.A.Go", val.Header);
        }
        [TestMethod]
        public void GetStringTransfer_NoTag_TypeTag()
        {
            var val = DefaultCacheNamedHelper.Default.GetStringTransfer(
                    new NamedInterceptorKey(typeof(B), typeof(B).GetMethod(nameof(B.NoTag))));

            Assert.IsInstanceOfType(val, typeof(NullStringTransfer));
        }
        [TestMethod]
        public void GetStringTransfer_Tag_TypeTag()
        {
            var val = DefaultCacheNamedHelper.Default.GetStringTransfer(
                    new NamedInterceptorKey(typeof(B), typeof(B).GetMethod(nameof(B.Tag))));

            Assert.IsInstanceOfType(val, typeof(NullStringTransfer1));
        }
        [TestMethod]
        public void GetStringTransfer_NoTag_NoTypeTag()
        {
            var val = DefaultCacheNamedHelper.Default.GetStringTransfer(
                    new NamedInterceptorKey(typeof(object), typeof(object).GetMethod(nameof(object.GetType))));

            Assert.IsInstanceOfType(val, typeof(DefaultStringTransfer));
        }
    }
}
