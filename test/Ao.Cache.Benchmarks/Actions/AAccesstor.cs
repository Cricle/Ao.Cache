using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    public class AAccesstor : IDataAccesstor<int, Student>
    {
        public AAccesstor(GetTime gt)
        {
            Gt = gt;
        }

        public GetTime Gt { get; set; }
        public Task<Student> FindAsync(int identity)
        {
            return Gt.Raw(identity);
        }
    }
}
