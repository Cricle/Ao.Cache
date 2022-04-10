using Microsoft.Extensions.DependencyInjection;
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
            where TWorkDataFinder : IDataAccesstor<TIdentity, TEntity>
        {
            var finder = scope.ServiceProvider.GetRequiredService<TWorkDataFinder>();
            return GetHashCacheFinder(scope, finder);
        }
        public HashCacheFinder<TIdentity, TEntity> GetHashCacheFinder<TIdentity, TEntity>(IServiceScope scope,
            IDataAccesstor<TIdentity, TEntity> finder)
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
            IDataAccesstor<TIdentity, TEntity> finder)
        {
            return new InternaHashCacheFinder<TIdentity, TEntity>(scope, database, finder);
        }
        public ListCacheFinder<TIdentity, TEntity> GetListCacheFinder<TIdentity, TEntity, TWorkDataFinder>(IServiceScope scope)
            where TWorkDataFinder : IDataAccesstor<TIdentity, TEntity>
        {
            var finder = scope.ServiceProvider.GetRequiredService<TWorkDataFinder>();
            return GetListCacheFinder(scope, finder);
        }
        public ListCacheFinder<TIdentity, TEntity> GetListCacheFinder<TIdentity, TEntity>(IServiceScope scope,
            IDataAccesstor<TIdentity, TEntity> finder)
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
            IDataAccesstor<TIdentity, TEntity> finder)
        {
            var f = new InternalListCacheFinder<TIdentity, TEntity>(scope, database, finder);
            f.Build();
            return f;
        }
        internal sealed class InternaHashCacheFinder<TIdentity, TEntity> : HashCacheFinder<TIdentity, TEntity>
        {
            private readonly IDataAccesstor<TIdentity, TEntity> finder;
            private readonly IServiceScope serviceScope;
            private readonly IDatabase database;

            public InternaHashCacheFinder(IServiceScope scope, IDatabase database, IDataAccesstor<TIdentity, TEntity> finder)
            {
                this.database = database;
                Debug.Assert(finder != null);
                this.serviceScope = scope;
                this.finder = finder;
            }

            public override void Dispose()
            {
                serviceScope.Dispose();
            }

            public override IDatabase GetDatabase()
            {
                return database;
            }

            protected override Task<TEntity> OnFindInDbAsync(TIdentity identity)
            {
                return finder.FindAsync(identity);
            }
        }
        internal sealed class InternalListCacheFinder<TIdentity, TEntity> : ListCacheFinder<TIdentity, TEntity>
        {
            private readonly IDataAccesstor<TIdentity, TEntity> finder;
            private readonly IServiceScope serviceScope;
            private readonly IDatabase database;

            public InternalListCacheFinder(IServiceScope scope, IDatabase database, IDataAccesstor<TIdentity, TEntity> finder)
            {
                Debug.Assert(finder != null);
                this.database = database;
                this.serviceScope = scope;
                this.finder = finder;
            }

            public override void Dispose()
            {
                serviceScope.Dispose();
            }

            public override IDatabase GetDatabase()
            {
                return database;
            }

            protected override Task<TEntity> OnFindInDbAsync(TIdentity identity)
            {
                return finder.FindAsync(identity);
            }
        }
    }

}