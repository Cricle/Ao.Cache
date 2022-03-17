﻿using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ao.Cache.Redis.Finders
{
    public class CacheFinderManager
    {
        public CacheFinderManager(IServiceScopeFactory serviceScopeFactory)
        {
            ServiceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public IServiceScopeFactory ServiceScopeFactory { get; }

        public HashCacheFinder<TIdentity, TEntity> GetHashCacheFinder<TIdentity, TEntity, TWorkDataFinder>(IServiceScope scope)
            where TWorkDataFinder : IWorkDataFinder<TIdentity, TEntity>
        {
            var finder = scope.ServiceProvider.GetRequiredService<TWorkDataFinder>();
            return GetHashCacheFinder(scope, finder);
        }
        public HashCacheFinder<TIdentity, TEntity> GetHashCacheFinder<TIdentity, TEntity>(IServiceScope scope,
            IWorkDataFinder<TIdentity, TEntity> finder)
        {
            var db = scope.ServiceProvider.GetService<IDatabase>();
            if (db == null)
            {
                var connect = scope.ServiceProvider.GetService<IConnectionMultiplexer>();
                if (connect == null)
                {
                    throw new InvalidOperationException($"Can't find service {typeof(IDatabase)} or {typeof(IConnectionMultiplexer)}");
                }
                db = connect.GetDatabase();
            }
            return new InternaHashCacheFinder<TIdentity, TEntity>(scope, db, finder);
        }

        public HashCacheFinder<TIdentity, TEntity> GetHashCacheFinder<TIdentity, TEntity>(IServiceScope scope,
            IDatabase database,
            IWorkDataFinder<TIdentity, TEntity> finder)
        {
            return new InternaHashCacheFinder<TIdentity, TEntity>(scope, database, finder);
        }
        public ListCacheFinder<TIdentity, TEntity> GetListCacheFinder<TIdentity, TEntity, TWorkDataFinder>(IServiceScope scope)
            where TWorkDataFinder : IWorkDataFinder<TIdentity, TEntity>
        {
            var finder = scope.ServiceProvider.GetRequiredService<TWorkDataFinder>();
            return GetListCacheFinder(scope, finder);
        }
        public ListCacheFinder<TIdentity, TEntity> GetListCacheFinder<TIdentity, TEntity>(IServiceScope scope,
            IWorkDataFinder<TIdentity, TEntity> finder)
        {
            var db = scope.ServiceProvider.GetService<IDatabase>();
            if (db == null)
            {
                var connect = scope.ServiceProvider.GetService<IConnectionMultiplexer>();
                if (connect == null)
                {
                    throw new InvalidOperationException($"Can't find service {typeof(IDatabase)} or {typeof(IConnectionMultiplexer)}");
                }
                db = connect.GetDatabase();
            }
            var f = new InternalListCacheFinder<TIdentity, TEntity>(scope, db, finder);
            f.Build();
            return f;
        }

        public ListCacheFinder<TIdentity, TEntity> GetListCacheFinder<TIdentity, TEntity>(IServiceScope scope,
            IDatabase database,
            IWorkDataFinder<TIdentity, TEntity> finder)
        {
            var f = new InternalListCacheFinder<TIdentity, TEntity>(scope, database, finder);
            f.Build();
            return f;
        }
        internal sealed class InternaHashCacheFinder<TIdentity, TEntity> : HashCacheFinder<TIdentity, TEntity>
        {
            private readonly IWorkDataFinder<TIdentity, TEntity> finder;
            private readonly IServiceScope serviceScope;

            public InternaHashCacheFinder(IServiceScope scope, IDatabase database, IWorkDataFinder<TIdentity, TEntity> finder)
                : base(database)
            {
                Debug.Assert(finder != null);
                this.serviceScope = scope;
                this.finder = finder;
            }

            public override void Dispose()
            {
                serviceScope.Dispose();
            }

            protected override Task<TEntity> OnFindInDbAsync(TIdentity identity)
            {
                return finder.FindAsync(identity);
            }
        }
        internal sealed class InternalListCacheFinder<TIdentity, TEntity> : ListCacheFinder<TIdentity, TEntity>
        {
            private readonly IWorkDataFinder<TIdentity, TEntity> finder;
            private readonly IServiceScope serviceScope;

            public InternalListCacheFinder(IServiceScope scope, IDatabase database, IWorkDataFinder<TIdentity, TEntity> finder)
                : base(database)
            {
                Debug.Assert(finder != null);
                this.serviceScope = scope;
                this.finder = finder;
            }

            public override void Dispose()
            {
                serviceScope.Dispose();
            }

            protected override Task<TEntity> OnFindInDbAsync(TIdentity identity)
            {
                return finder.FindAsync(identity);
            }
        }
    }

}