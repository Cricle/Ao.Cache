using System;
using System.Collections.Generic;

namespace Ao.Cache
{
    public interface IDataFinderFactory
    {
        IDataFinder<TIdentity, TEntity> Create<TIdentity, TEntity>(IDataAccesstor<TIdentity, TEntity> accesstor);
    }
    public interface IStringMaker
    {
        string ToString<T>(T input);
    }
    public class DefaultStringMaker :Dictionary<Type,Func<object,string>>, IStringMaker
    {
        public string ToString<T>(T input)
        {
            if (TryGetValue(typeof(T),out var trans))
            {
                return trans(input);
            }
            return input?.ToString();
        }
    }
}
