using Ao.Cache.Core.Annotations;
using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    [CacheProxy]
    public class GetTimeCt
    {
        [CacheProxyMethod(Inline = false)]
        public virtual async Task<Student> NowTime(int id,object a,double b)
        {
            await Task.Yield();
            return new Student { Id = id };
        }
        [CacheProxyMethod(Inline =false)]
        public virtual Student NowTimeSync(int id, object a, double b)
        {
            return new Student { Id = id };
        }
        public async Task<Student> Raw(int id, object a, double b)
        {
            await Task.Yield();
            return new Student { Id = id };
        }
        public Student RawSync(int id, object a, double b)
        {
            return new Student { Id = id };
        }
    }
}
