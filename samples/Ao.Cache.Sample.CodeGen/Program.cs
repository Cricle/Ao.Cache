using Ao.Cache.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using dsadsa;
using Ao.Cache.Sample.CodeGen;

namespace Ao.Cache.Sample.CodeGen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddInMemoryFinder();
            services.AddScoped<Student>();
            services.AddScoped<StudentProxy>();
            var provider = services.BuildServiceProvider();
            var finder = provider.GetRequiredService<StudentProxy>();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(finder.Get<int>(1));
            }

        }
    }
    public interface IStudent
    {
        void Run();

        [CacheProxyMethod]
        int? Get<T>(int? a);

        [CacheProxyMethod]
        int Get1(A a);

        [CacheProxyMethod]
        Task<int> Get2(A a);

        [CacheProxyMethod]
        ValueTask<int> Get3(A a);
    }
    [CacheProxy(ProxyType = typeof(Student))]
    public class Student : IStudent
    {
        [CacheProxyMethod]
        public virtual int? Get<T>(int? a)
        {
            return Random.Shared.Next(0, 9999) + a.GetHashCode();
        }

        [CacheProxyMethod]
        public virtual int Get1(A a)
        {
            throw new NotImplementedException();
        }

        [CacheProxyMethod]
        public virtual Task<int> Get2(A a)
        {
            throw new NotImplementedException();
        }

        [CacheProxyMethod]
        public virtual ValueTask<int> Get3(A a)
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }

    public struct A
    {

    }
    [DataAccesstor(NameSpace ="dsadsa")]
    public class TestDataAccesstor : IDataAccesstor<A, int?>
    {
        public Task<int?> FindAsync(A identity)
        {
            return Task.FromResult<int?>(identity.GetHashCode() + Random.Shared.Next(0,9999));
        }
    }
}