using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Test
{
    [TestClass]
    public class ExpressionHasherTest
    {
        [ExcludeFromCodeCoverage]
        class Student
        {
            public string? Name { get; set; }

            public string Convert(int a, int[] arr)
            {
                throw null;
            }
        }
        [TestMethod]
        public void HashCreate()
        {
            Expression<Func<Student, string?>> exp1 = x => x.Name + "123" + x.Convert(2, new int[] { 1, 2, 3 });
            Expression<Func<Student, string?>> exp2 = x => x.Name + "123" + x.Convert(2, new int[] { 1, 2, 3 });

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            Assert.AreEqual(h1, h2);
        }
    }
}
