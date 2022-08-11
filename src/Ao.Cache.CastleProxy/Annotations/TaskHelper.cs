using System.Threading.Tasks;

namespace Ao.Cache.CastleProxy.Annotations
{
    internal static class TaskHelper
    {
#if NET452
        public static readonly Task ComplatedTask = Task.FromResult(false);
#else
        public static readonly Task ComplatedTask = Task.CompletedTask;
#endif
    }
}
