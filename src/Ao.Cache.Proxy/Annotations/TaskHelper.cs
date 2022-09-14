using System.Threading.Tasks;

namespace Ao.Cache.Proxy.Annotations
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
