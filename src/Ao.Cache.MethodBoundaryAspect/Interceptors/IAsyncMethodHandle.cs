using MethodBoundaryAspect.Fody.Attributes;
using System.Threading.Tasks;

namespace Ao.Cache.MethodBoundaryAspect.Interceptors
{
    public interface IAsyncMethodHandle
    {
        Task<T> HandleEntryAsync<T>(MethodExecutionArgs arg, T old);

        Task<T> HandleExitAsync<T>(MethodExecutionArgs arg, T old);

        Task<T> HandleExceptionAsync<T>(MethodExecutionArgs arg, T old);
    }

}
