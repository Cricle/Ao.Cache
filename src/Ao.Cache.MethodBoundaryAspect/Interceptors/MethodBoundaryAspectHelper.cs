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


        public static bool AsyncIntercept(MethodExecutionArgs arg, IAsyncMethodHandle handle, MethodBoundaryMethods method)
        {
            var rt = GetTaskResultType(((MethodInfo)arg.Method).ReturnType);
            if (rt.Case == MethodReturnCase.TaskResult)
            {
                var del = DynamicTaskProxy.GetDelegate(rt.ReturnGenericType);
                arg.ReturnValue = del(handle,
                    arg,
                    method);
                return true;
            }
            return false;
        }
    }

}
