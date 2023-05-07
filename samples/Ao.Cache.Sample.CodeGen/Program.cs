
using Ao.Cache.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Ao.Cache.Gen;
using System.Diagnostics;

[assembly: EnableCacheAutoServiceRegist]

namespace Ao.Cache.Sample.CodeGen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var services = new ServiceCollection();
            //services.AddInMemoryFinder();
            //services.AddScoped<Student>();
            //services.AddScoped<StudentProxy>();
            //var provider = services.BuildServiceProvider();
            //var finder = provider.GetRequiredService<StudentProxy>();
            //var creator = provider.GetRequiredService<ICacheHelperCreator>();
            //var c = provider.GetRequiredService<Student>();
            //var ax = new A();
            //_ = finder.Get<int>(0);
            //_ = c.Get2(ax).Result;
            //var gc = GC.GetTotalMemory(true);
            //var sw = Stopwatch.GetTimestamp();
            //for (int i = 0; i < 1_000_000; i++)
            //{
            //    _ = finder.Get2(ax).Result;
            //}
            //Console.WriteLine(new TimeSpan(Stopwatch.GetTimestamp() - sw));
            //Console.WriteLine($"{(GC.GetTotalMemory(false) - gc) / 1024 / 1024.0}");
        }
    }
    //[CacheProxy(ProxyType = typeof(Student), ProxyAll = true)]
    public interface IStudent
    {
        void Run();

        int? Get<T>(int? a);

        int? Get1(A a);

        Task<int?> Get2(A a);
    }
    [CacheProxy(Head = "test")]
    public class Student:IStudent
    {
        public Student()
        {

        }
        public virtual int? Get<T>()
        {
            return Random.Shared.Next(0, 9999);
        }
        public virtual int? Get<T>(int? a)
        {
            return Random.Shared.Next(0, 9999) + a.GetHashCode();
        }

        public virtual int? Get1(A a)
        {
            return Random.Shared.Next(0, 9999) + a.GetHashCode();
        }
        public virtual int? Getmul(A a, int b, DateTime c)
        {
            return Random.Shared.Next(0, 9999) + a.GetHashCode();
        }
        public virtual async Task<int?> Get2(A a)
        {
            await Task.Yield();
            return Random.Shared.Next(0, 9999) + a.GetHashCode();
        }
        public virtual ValueTask<int> Get3(A a)
        {
            return new ValueTask<int>(Random.Shared.Next(0, 9999) + a.GetHashCode());
        }

        public virtual void Run()
        {
            throw new NotImplementedException();
        }
    }

    public struct A
    {
        public override int GetHashCode()
        {
            return 111;
        }
        public override string ToString()
        {
            return GetHashCode().ToString();
        }
    }
    [DataAccesstor(NameSpace = "dsadsa")]
    public class TestDataAccesstor : IDataAccesstor<A, int?>
    {
        public Task<int?> FindAsync(A identity)
        {
            return Task.FromResult<int?>(identity.GetHashCode() + Random.Shared.Next(0, 9999));
        }
    }

}