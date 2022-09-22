using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Ao.Cache.MethodBoundaryAspect.Interceptors
{
    public static class DynamicTaskProxy
    {
        private static readonly MethodInfo withResultMethodInfo = typeof(DynamicTaskProxy).GetMethod(nameof(WithResult), BindingFlags.Static | BindingFlags.Public);
        private static readonly Dictionary<Type, Delegate> g = new Dictionary<Type, Delegate>();
        private static readonly object syncRoot = new object();

        public static Delegate GetDelegate(Type type)
        {
            if (!g.TryGetValue(type, out var del))
            {
                lock (syncRoot)
                {
                    if (!g.TryGetValue(type, out del))
                    {
                        var t = typeof(Func<,,,,>).MakeGenericType(
                            typeof(IAsyncMethodHandle),
                            typeof(MethodExecutionArgs),
                            typeof(Task<>).MakeGenericType(type),
                            typeof(Func<,,,>).MakeGenericType(
                                typeof(IAsyncMethodHandle),
                                typeof(MethodExecutionArgs),
                                type,
                                typeof(Task<>).MakeGenericType(type)),
                                typeof(Task<>).MakeGenericType(type));
                        del = Delegate.CreateDelegate(t,
                            withResultMethodInfo.MakeGenericMethod(type));
                        g.Add(type, del);
                    }
                }
            }
            return del;
        }

        public static async Task<T> WithResult<T>(IAsyncMethodHandle handle,
            MethodExecutionArgs arg,
            Task<T> left,
            Func<IAsyncMethodHandle, MethodExecutionArgs, T, Task<T>> right)
        {
            var old = default(T);
            if (left != null)
            {
                old = await left;
            }
            return await right(handle, arg, old);
        }
    }

}
