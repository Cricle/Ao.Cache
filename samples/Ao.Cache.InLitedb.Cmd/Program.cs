using Ao.Cache.Serizlier.TextJson;
using LiteDB;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ao.Cache.InLitedb.Cmd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }
        static async Task RunAsync()
        {
            using (var litedb = new LiteDatabase("a.db"))
            {
                var d = litedb.GetCacheCollection();
                d.EnsureIndex();
                var finder = new LitedbCacheFactory(litedb,d, TextJsonEntityConvertor.Default);
                var f = finder.Create(new DataAsstor());
                var q = await f.FindAsync(123);
                Console.WriteLine(q);
                q = await f.FindAsync(123);
                Console.WriteLine(q);
                await f.DeleteAsync(123);
                q = await f.FindAsync(123);
                Console.WriteLine(q);
            }
        }
    }
    public class DataAsstor : IDataAccesstor<long, Student>
    {
        public Task<Student> FindAsync(long identity)
        {
            var rand = new Random();
            return Task.FromResult(new Student
            {
                Class = "c" + rand.Next(10000, 9999999),
                Idx = identity,
                Name = "aaa" + rand.Next(10000, 9999999)
            });
        }
    }
    public class Student
    {
        public long Idx { get; set; }

        public string Name { get; set; }

        public string Class { get; set; }

        public override string ToString()
        {
            return $"{Idx},{Name},{Class}";
        }
    }
}