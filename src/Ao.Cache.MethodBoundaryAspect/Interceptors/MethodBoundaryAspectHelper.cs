using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Ao.Cache.MethodBoundaryAspect.Interceptors
{
    public enum MethodReturnCase
    {
        Other=0,
        Task = 1,
        TaskResult = Task<<1,
    }
    public static class MethodBoundaryAspectHelper
    {
        class MethodReturnInfo
        {
            public MethodReturnCase Case;

            public Type ReturnGenericType;
        }

        private static readonly Dictionary<Type, MethodReturnInfo> isTaskWithSouce = new Dictionary<Type, MethodReturnInfo>();
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

        
        public static bool AsyncIntercept(MethodExecutionArgs arg, IAsyncMethodHandle handle, MethodBoundaryMethods method, MethodReturnCase @case= MethodReturnCase.TaskResult)
        {
            var rt = GetTaskResultType(((MethodInfo)arg.Method).ReturnType);
            if ((rt.Case & @case)!=0)
            {
                if (rt.Case == MethodReturnCase.Task)
                {
                    if (arg.ReturnValue is Task tsk)
                    {
                        switch (method)
                        {
                            case MethodBoundaryMethods.Entry:
                                arg.ReturnValue = tsk.ContinueWith(_ => handle.HandleEntryAsync<int>(arg, default));
                                break;
                            case MethodBoundaryMethods.Exit:
                                arg.ReturnValue = tsk.ContinueWith(_ => handle.HandleExitAsync<int>(arg, default));
                                break;
                            case MethodBoundaryMethods.Exception:
                                arg.ReturnValue = tsk.ContinueWith(_ => handle.HandleExceptionAsync<int>(arg, default));
                                break;
                            default:
                                throw new NotSupportedException(method.ToString());
                        }
                    }
                    else
                    {
                        switch (method)
                        {
                            case MethodBoundaryMethods.Entry:
                                arg.ReturnValue = handle.HandleEntryAsync<int>(arg, default);
                                break;
                            case MethodBoundaryMethods.Exit:
                                arg.ReturnValue = handle.HandleExitAsync<int>(arg, default);
                                break;
                            case MethodBoundaryMethods.Exception:
                                arg.ReturnValue = handle.HandleExceptionAsync<int>(arg, default);
                                break;
                            default:
                                throw new NotSupportedException(method.ToString());
                        }
                    }
                }
                else
                {
                    var del = DynamicTaskProxy.GetDelegate(rt.ReturnGenericType);
                    arg.ReturnValue = del(handle,
                        arg,
                        method);
                }
                return true;
            }
            return false;
        }
    }

}
