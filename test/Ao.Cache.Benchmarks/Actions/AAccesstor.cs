using System.Threading.Tasks;

namespace Ao.Cache.Benchmarks.Actions
{
    public class AAccesstor : IDataAccesstor<int, Student>
    {
        public AAccesstor(GetTimeCt gt)
        {
            Gt = gt;
        }

        public GetTimeCt Gt { get; set; }

        public Task<Student> FindAsync(int identity)
        {
            return Gt.Raw(identity);
        }
    }
}
