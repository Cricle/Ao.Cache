using Ao.Cache.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Ao.Cache.Gen;

namespace Ao.Cache.Sample.CodeGen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddScoped<DataFinders>().AddTestDataAccesstor();
            services.AddInMemoryFinder();
            var provider = services.BuildServiceProvider();
            var finder = provider.GetRequiredService<DataFinders>().GetTest();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(finder.FindAsync(new A()).Result);
            }
        }
    }
    public struct A
    {

    }
    [DataAccesstor]
    public class TestDataAccesstor : IDataAccesstor<A, int?>
    {
        public Task<int?> FindAsync(A identity)
        {
            return Task.FromResult<int?>(identity.GetHashCode() + Random.Shared.Next(0,9999));
        }
    }
}
