using Ao.Cache.InRedis.HashList.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ao.Cache.InRedis.HashList.Test.Converters
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
            Assert.AreNotEqual(0, val.Length());

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
