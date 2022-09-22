using Ao.Cache.MethodBoundaryAspect.Interceptors;
using Ao.Cache.Proxy.Annotations;
using Ao.Cache.Proxy.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    class GetTimeCt
    {
        [AutoCache]
        [CacheInterceptor] 
        [AutoCacheOptions(CanRenewal = false)]
        public virtual async Task<Student> NowTime(int id)
        {
            return await Raw(id);
        }
        [AutoCache]
        [CacheInterceptor]
        [AutoCacheOptions(CanRenewal = false)]
        public virtual async Task<AutoCacheResult<Student>> NowTime1(int id)
        {
            return new AutoCacheResult<Student> { RawData = await Raw(id) };
        }
        public async Task<Student> Raw(int id)
        {
            return new Student { Id = id };
            //using (var dbc = dbContextFactory.CreateDbContext())
            //{
            //    return await dbc.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            //}
        }
    }
    public class GetTime
    {
        [AutoCache]
        [AutoCacheOptions(CanRenewal = false)]
        public virtual async Task<AutoCacheResult<Student>> NowTime(int id)
        {
            return new AutoCacheResult<Student> { RawData = await Raw(id) };
        }

        [AutoCache]
        [AutoCacheOptions(CanRenewal = false)]
        public virtual async Task<Student> NowTime1(int id)
        {
            return await Raw(id);
        }

        public async Task<Student> Raw(int id)
        {
            return new Student { Id = id };
            //using (var dbc = dbContextFactory.CreateDbContext())
            //{
            //    return await dbc.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            //}
        }
    }
}
