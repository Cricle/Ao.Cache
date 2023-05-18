using System;
using System.Collections.Generic;
namespace Ao.Cache
{
    public interface ISyncDataFinderFactory
    {
        ISyncDataFinder<TIdentity,TEntity> CreateSync<TIdentity,TEntity>();

        ISyncWithDataFinder<TIdentity, TEntity> CreateSync<TIdentity,TEntity>(ISyncDataAccesstor<TIdentity, TEntity> accesstor);
    }
    public interface IDataFinderFactory
    {
        IDataFinder<TIdentity,TEntity> Create<TIdentity,TEntity>();

        IWithDataFinder<TIdentity, TEntity> Create<TIdentity,TEntity>(IDataAccesstor<TIdentity, TEntity> accesstor);
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
