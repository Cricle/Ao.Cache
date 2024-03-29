﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ao.Cache
{
    public interface IWithDataFinderOptions<TIdentity, TEntity>
    {
        IDataFinderOptions<TIdentity, TEntity> Options { get; set; }
    }
    public interface ISyncWithBatchDataFinder<TIdentity, TEntity> : ISyncBatchDataFinder<TIdentity, TEntity>, ISyncWithBatchDataAccesstorFinder<TIdentity, TEntity>
    {
    }
    public interface IWithBatchDataFinder<TIdentity, TEntity> : IBatchDataFinder<TIdentity, TEntity>, IWithBatchDataAccesstorFinder<TIdentity, TEntity>
    {
    }
    public interface IBatchDataFinder<TIdentity, TEntity> : IWithDataFinderOptions<TIdentity, TEntity>, IBatchCacheFinder<TIdentity, TEntity>, IBatchPhysicalFinder<TIdentity, TEntity>, IBatchRenewalable<TIdentity>
    {
        Task<long> DeleteAsync(IReadOnlyList<TIdentity> identity);

        Task<IDictionary<TIdentity, bool>> ExistsAsync(IReadOnlyList<TIdentity> identity);
    }
    public interface ISyncBatchDataFinder<TIdentity, TEntity> : IWithDataFinderOptions<TIdentity, TEntity>, ISyncBatchCacheFinder<TIdentity, TEntity>, ISyncBatchPhysicalFinder<TIdentity, TEntity>, ISyncBatchRenewalable<TIdentity>
    {
        long Delete(IReadOnlyList<TIdentity> identity);

        IDictionary<TIdentity, bool> Exists(IReadOnlyList<TIdentity> identity);
    }
}
