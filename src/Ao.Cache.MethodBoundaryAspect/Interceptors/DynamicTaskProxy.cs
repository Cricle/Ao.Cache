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
        private static readonly Dictionary<Type, Func<IAsyncMethodHandle, MethodExecutionArgs, object,object,object>> g = 
            new Dictionary<Type, Func<IAsyncMethodHandle, MethodExecutionArgs, object, object, object>>();
        private static readonly object syncRoot = new object();

        public static Func<IAsyncMethodHandle, MethodExecutionArgs, object, object, object> GetDelegate(Type type)
        {
            if (!g.TryGetValue(type, out var del))
            {
                lock (syncRoot)
                {
                    if (!g.TryGetValue(type, out del))
                    {
                        var par0 = Expression.Parameter(typeof(IAsyncMethodHandle));
                        var par1 = Expression.Parameter(typeof(MethodExecutionArgs));
                        var par2 = Expression.Parameter(typeof(object));
                        var par3 = Expression.Parameter(typeof(object));
                        var funcType = typeof(Func<,,,>).MakeGenericType(
                                typeof(IAsyncMethodHandle),
                                typeof(MethodExecutionArgs),
                                type,
                                typeof(Task<>).MakeGenericType(type));
                        del = Expression.Lambda<Func<IAsyncMethodHandle, MethodExecutionArgs, object, object, object>>(
                            Expression.Call(null,
                                withResultMethodInfo.MakeGenericMethod(type),
                                par0,
                                par1,
                                Expression.Convert(par2, typeof(Task<>).MakeGenericType(type)),
                                Expression.Convert(par3, funcType)),
                            par0, par1, par2, par3).Compile();
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
