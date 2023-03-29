
using Ao.Cache.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Ao.Cache.Gen;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;
using System.Reflection;

[assembly:EnableCacheAutoServiceRegist]

namespace Ao.Cache.Sample.CodeGen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddInMemoryFinder();
            services.AddScoped<Student>();
            services.AddScoped<StudentProxya>();
            var provider = services.BuildServiceProvider();
            var mem = provider.GetRequiredService<IMemoryCache>();
            var finder = provider.GetRequiredService<StudentProxya>();
            var c = provider.GetRequiredService<Student>();
            var ax = new A();
            _ = finder.Get2(ax).Result;
            _ = c.Get2(ax).Result;
            var gc = GC.GetTotalMemory(true);
            var sw = Stopwatch.GetTimestamp();
            for (int i = 0; i < 1_000_000; i++)
            {
                _ = finder.Get2(ax).Result;
            }
            Console.WriteLine(new TimeSpan(Stopwatch.GetTimestamp() - sw));
            Console.WriteLine($"{(GC.GetTotalMemory(false) - gc) / 1024 / 1024.0}");
        }
    }
    //[CacheProxy(ProxyType = typeof(Student), ProxyAll = true)]
    public interface IStudent
    {
        void Run();

        int? Get<T>(int? a);

        int? Get1(A a);

        Task<int?> Get2(A a);

        ValueTask<int> Get3(A a);
    }
    [CacheProxy(Head ="test")]
    public class Student : IStudent
    {
        public virtual int? Get<T>(int? a)
        {
            return Random.Shared.Next(0, 9999) + a.GetHashCode();
        }

        public virtual int? Get1(A a)
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
    [DataAccesstor(NameSpace ="dsadsa")]
    public class TestDataAccesstor : IDataAccesstor<A, int?>
    {
        public Task<int?> FindAsync(A identity)
        {
            return Task.FromResult<int?>(identity.GetHashCode() + Random.Shared.Next(0,9999));
        }
    }
    public class StudentProxya : Ao.Cache.Sample.CodeGen.Student
    {

        protected readonly IDataFinderFactory _factory;

        public StudentProxya(IDataFinderFactory factory)
        {

            _factory = factory ?? throw new System.ArgumentNullException(nameof(factory));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int? Get<T>(int? a)
        {

            var finder = _factory.Create(new DelegateDataAccesstor<int?, int?>(identity => base.Get<T>(identity)));

            finder.Options.WithRenew(false);

            finder.Options.WithHead("test.Get(a)");

            return finder.FindAsync(a).GetAwaiter().GetResult();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int? Get1(Ao.Cache.Sample.CodeGen.A a)
        {

            var finder = _factory.Create(new DelegateDataAccesstor<Ao.Cache.Sample.CodeGen.A, int?>(identity => base.Get1(identity)));

            finder.Options.WithRenew(false);

            finder.Options.WithHead("test.Get1(a)");

            return finder.FindAsync(a).GetAwaiter().GetResult();
        }


        public override System.Threading.Tasks.Task<int?> Get2(Ao.Cache.Sample.CodeGen.A a)
        {

            var finder = _factory.Create(new DelegateDataAccesstor<Ao.Cache.Sample.CodeGen.A, int?>(base.Get2));

            finder.Options.WithRenew(false);

            finder.Options.WithHead("test.Get2(a)");

            return finder.FindAsync(a);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override async System.Threading.Tasks.ValueTask<int> Get3(Ao.Cache.Sample.CodeGen.A a)
        {

            var finder = _factory.Create(new DelegateDataAccesstor<Ao.Cache.Sample.CodeGen.A, int>(base.Get3));

            finder.Options.WithRenew(false);

            finder.Options.WithHead("test.Get3(a)");

            return await finder.FindAsync(a);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Run()
        {

            base.Run();
        }

    }

}