using Ao.Cache.Core.Annotations;
using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    [CacheProxy]
    public class GetTimeCt
    {
        [CacheProxyMethod]
        public virtual async Task<Student> NowTime(int id)
        {
            return await Raw(id);
        }
        public async Task<Student> Raw(int id)
        {
            return new Student { Id = id };
        }
    }
}
