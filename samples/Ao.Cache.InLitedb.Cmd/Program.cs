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
                var coll = litedb.GetCollection<CacheStudent>("students", BsonAutoId.Int64);
                coll.EnsureIndex(x => x.Idx);
                var finder = new StudentLitedbCacheFinder(coll);
                var del = finder.DeleteInvalidRows();
                Console.WriteLine($"Deleted {del} rows");
                for (int i = 0; i < 10_000; i++)
                {
                    var ds = await finder.FindAsync(100);
                    Console.WriteLine(ds.ToString());
                    await Task.Delay(1000);
                }
            }
        }
    }
    public class StudentLitedbCacheFinder : LitedbCacheFinder<long, Student, CacheStudent>
    {
        public StudentLitedbCacheFinder(ILiteCollection<CacheStudent> collection) : base(collection)
        {
        }
        public override TimeSpan? GetCacheTime(long identity, Student entity)
        {
            return TimeSpan.FromSeconds(2);
        }
        protected override Expression<Func<CacheStudent, Student>> GetSelect(long identity)
        {
            return x => new CacheStudent
            {
                Idx = x.Idx,
                Class = x.Class,
                Name = x.Name,
                Time = DateTime.Now
            };
        }

        protected override Expression<Func<CacheStudent, bool>> GetWhere(long identity)
        {
            return x => x.Idx == identity;
        }

        protected override Task<Student> OnFindInDbAsync(long identity)
        {
            var rand = new Random();
            return Task.FromResult(new Student
            {
                Class = "c" + rand.Next(10000, 9999999),
                Idx = identity,
                Name = "aaa" + rand.Next(10000, 9999999)
            });
        }

        protected override CacheStudent ToCollectionEntity(long identity, Student entry)
        {
            return new CacheStudent
            {
                Class = entry.Class,
                Time = DateTime.Now,
                ExpirationTime = DateTime.Now,
                Idx = identity,
                Name = entry.Name
            };
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
    public class CacheStudent : Student, ILiteCacheEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public DateTime Time { get; set; }

        public DateTime? ExpirationTime { get; set; }
    }
}