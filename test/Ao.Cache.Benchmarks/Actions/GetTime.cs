using Ao.Cache.CastleProxy.Annotations;
using Ao.Cache.CastleProxy.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    public class GetTime
    {
        private readonly IDbContextFactory<StudentDbContext> dbContextFactory;

        public GetTime(IDbContextFactory<StudentDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        [AutoCache]
        [AutoCacheOptions(CanRenewal = false)]
        public virtual async Task<AutoCacheResult<Student>> NowTime(int id)
        {
            return new AutoCacheResult<Student> { RawData =await Raw(id) };
        }

        [AutoCache]
        [AutoCacheOptions(CanRenewal =false)]
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
