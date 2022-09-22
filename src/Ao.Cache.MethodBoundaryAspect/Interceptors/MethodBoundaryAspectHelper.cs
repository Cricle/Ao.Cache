using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Ao.Cache.MethodBoundaryAspect.Interceptors
{
    public static class MethodBoundaryAspectHelper
    {
        class HandleMerge
        {
            public Delegate Entry;

            public Delegate Exit;

            public Delegate Exception;
        }
        enum MethodReturnCase
        {
            Other = 0,
            Task = 1,
            TaskResult = 2
        }
        class MethodReturnInfo
        {
            public MethodReturnCase Case;

            public Type ReturnGenericType;
        }
        private static readonly MethodInfo entryMethodInfo =
            typeof(IAsyncMethodHandle).GetMethod(nameof(IAsyncMethodHandle.HandleEntryAsync));
        private static readonly MethodInfo exitMethodInfo =
            typeof(IAsyncMethodHandle).GetMethod(nameof(IAsyncMethodHandle.HandleExitAsync));
        private static readonly MethodInfo exceptionMethodInfo =
            typeof(IAsyncMethodHandle).GetMethod(nameof(IAsyncMethodHandle.HandleExceptionAsync));

        private static readonly Dictionary<Type, HandleMerge> handleDelegate = new Dictionary<Type, HandleMerge>();
        private static readonly Dictionary<Type, MethodReturnInfo> isTaskWithSouce = new Dictionary<Type, MethodReturnInfo>();
        private static readonly object syncRootHandleDelegate = new object();
        private static readonly object syncRootisTaskWithSouce = new object();

        private static MethodReturnInfo GetTaskResultType(Type type)
        {
            if (!isTaskWithSouce.TryGetValue(type, out var ifo))
            {
                lock (syncRootisTaskWithSouce)
                {
                    if (!isTaskWithSouce.TryGetValue(type, out ifo))
                    {
                        var @case = MethodReturnCase.Other;
                        if (type == typeof(Task))
                        {
                            @case = MethodReturnCase.Task;
                        }
                        else if (typeof(Task).IsAssignableFrom(type) &&
                            type.IsGenericType)
                        {
                            @case = MethodReturnCase.TaskResult;
                        }
                        else
                        {
                            @case = MethodReturnCase.Other;
                        }
                        ifo = new MethodReturnInfo
                        {
                            Case = @case,
                            ReturnGenericType = @case == MethodReturnCase.TaskResult ? type.GenericTypeArguments[0] : null
                        };
                        isTaskWithSouce[type] = ifo;
                    }
                }
            }
            return ifo;
        }

        private static HandleMerge GetHandleDelegate(Type type)
        {
            if (!handleDelegate.TryGetValue(type, out var merge))
            {
                lock (syncRootHandleDelegate)
                {
                    if (!handleDelegate.TryGetValue(type, out merge))
                    {
                        merge = new HandleMerge
                        {
                            Entry = CreateHandleDelegate(type, entryMethodInfo),
                            Exit = CreateHandleDelegate(type, exitMethodInfo),
                            Exception = CreateHandleDelegate(type, exceptionMethodInfo),
                        };
                        handleDelegate[type] = merge;
                    }
                }
            }
            return merge;
        }
        private static Delegate CreateHandleDelegate(Type type, MethodInfo method)
        {
            var genMethod = method.MakeGenericMethod(type);
            var par0 = Expression.Parameter(typeof(IAsyncMethodHandle));
            var par1 = Expression.Parameter(typeof(MethodExecutionArgs));
            var par2 = Expression.Parameter(type);
            return Expression.Lambda(Expression.Call(par0, genMethod, par1, par2),
                par0, par1, par2)
                .Compile();
        }

        public static bool AsyncIntercept(MethodExecutionArgs arg, IAsyncMethodHandle handle, MethodBoundaryMethods method)
        {
            var rt = GetTaskResultType(((MethodInfo)arg.Method).ReturnType);
            if (rt.Case == MethodReturnCase.TaskResult)
            {
                var merge = GetHandleDelegate(rt.ReturnGenericType);
                var del = DynamicTaskProxy.GetDelegate(rt.ReturnGenericType);
                var mergeDel = merge.Entry;
                switch (method)
                {
                    case MethodBoundaryMethods.Exit:
                        mergeDel = merge.Exit;
                        break;
                    case MethodBoundaryMethods.Exception:
                        mergeDel = merge.Exception;
                        break;
                    case MethodBoundaryMethods.Entry:
                    default:
                        break;
                }
                arg.ReturnValue = del(handle,
                    arg,
                    arg.ReturnValue,
                    mergeDel);
                return true;
            }
            return false;
        }
    }

}
