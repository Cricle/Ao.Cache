using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Ao.Cache.MethodBoundaryAspect.Interceptors
{
    public static class DynamicTaskProxy
    {
        private static readonly MethodInfo withResultMethodInfo = typeof(DynamicTaskProxy).GetMethod(nameof(WithResult), BindingFlags.Static | BindingFlags.Public);
        private static readonly Dictionary<Type, Func<IAsyncMethodHandle, MethodExecutionArgs, MethodBoundaryMethods, object>> g =
            new Dictionary<Type, Func<IAsyncMethodHandle, MethodExecutionArgs, MethodBoundaryMethods, object>>();
        private static readonly object syncRoot = new object();

        public static Func<IAsyncMethodHandle, MethodExecutionArgs, MethodBoundaryMethods, object> GetDelegate(Type type)
        {
            if (!g.TryGetValue(type, out var del))
            {
                lock (syncRoot)
                {
                    if (!g.TryGetValue(type, out del))
                    {
                        var par0 = Expression.Parameter(typeof(IAsyncMethodHandle));
                        var par1 = Expression.Parameter(typeof(MethodExecutionArgs));
                        var par2 = Expression.Parameter(typeof(MethodBoundaryMethods));
                        del = Expression.Lambda<Func<IAsyncMethodHandle, MethodExecutionArgs, MethodBoundaryMethods, object>>(
                            Expression.Call(null,
                                withResultMethodInfo.MakeGenericMethod(type),
                                par0,
                                par1,
                                par2),
                            par0, par1, par2).Compile();
                        g.Add(type, del);
                    }
                }
            }
            return del;
        }
        public static async Task<T> WithResult<T>(IAsyncMethodHandle handle,
            MethodExecutionArgs arg,
            MethodBoundaryMethods method)
        {
            var old = default(T);
            if (arg.ReturnValue is Task<T> taskT)
            {
                old = await taskT;
            }
            switch (method)
            {
                case MethodBoundaryMethods.Entry:
                    return await handle.HandleEntryAsync(arg, old);
                case MethodBoundaryMethods.Exit:
                    return await handle.HandleExitAsync(arg, old);
                case MethodBoundaryMethods.Exception:
                    return await handle.HandleExceptionAsync(arg, old);
                default:
                    throw new NotSupportedException(method.ToString());
            }
        }
    }

}
