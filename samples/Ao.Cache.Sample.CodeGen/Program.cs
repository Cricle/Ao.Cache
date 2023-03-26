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
                //.GetTest();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(finder.FindAsync(1).Result);
            }
        }
    }
    [DataAccesstor]
    public class TestDataAccesstor : IDataAccesstor<int, int?>
    {
        public Task<int?> FindAsync(int identity)
        {
            return Task.FromResult<int?>(identity + Random.Shared.Next(0,9999));
        }
    }
}
