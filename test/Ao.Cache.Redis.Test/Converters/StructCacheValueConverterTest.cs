using Ao.Cache.Redis.Converters;
using Ao.ObjectDesign;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ao.Cache.Redis.Test.Converters
{
    struct Student
    {
        public int Id;

        public string Name;
    }
    class Box
    {
        public Student Student { get; set; }
    }
    [TestClass]
    public class StructCacheValueConverterTest
    {       
        [TestMethod]
        public void ConvertWithBytes()
        {
            var s = new Student { Id = 1, Name = "asdasda" };
            var box = new Box { Student = s };
            var inst = StructCacheValueConverter.Instance;
            var val = inst.Convert(box, s, new CacheColumn
            {
                Property = typeof(Box).GetProperty("Student")
            });
            Assert.AreNotEqual(0,val.Length());

            var entry = inst.ConvertBack(val, new CacheColumn
            {
                Property = typeof(Box).GetProperty("Student")
            });
            Assert.IsInstanceOfType(entry, typeof(Student));
            var stu = (Student)entry;
            Assert.AreEqual(1, stu.Id);
            Assert.AreEqual("asdasda", stu.Name);
        }
    }
}
