using System.Linq.Expressions;
using System.Reflection;
using System;

namespace Ao.Cache
{
    public interface ICacheHelper<TReturn>
    {
        IDataFinder<string, TReturn> GetFinder(Type instanceType, MethodInfo method);

        IDataFinder<string, TReturn> GetFinder(Expression<Func<TReturn>> exp);

        ISyncDataFinder<string, TReturn> GetFinderSync(Type instanceType, MethodInfo method);

        ISyncDataFinder<string, TReturn> GetFinderSync(Expression<Func<TReturn>> exp);
    }

}
