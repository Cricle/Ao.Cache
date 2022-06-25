﻿using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ao.Cache.Redis.Finders
{
    public class DefaultHashCacheFinder<TIdentity, TEntry> : HashCacheFinder<TIdentity, TEntry>
    {
        public DefaultHashCacheFinder(IDatabase database, IDataAccesstor<TIdentity, TEntry> dataAccesstor)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            DataAccesstor = dataAccesstor ?? throw new ArgumentNullException(nameof(dataAccesstor));
            Build();
        }

        public IDatabase Database { get; }

        public IDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }

        protected override Task<TEntry> OnFindInDbAsync(TIdentity identity)
        {
            return DataAccesstor.FindAsync(identity);
        }
        protected override TimeSpan? GetCacheTime(TIdentity identity, TEntry entity)
        {
            return DataAccesstor.GetCacheTime(identity, entity);
        }

        public override IDatabase GetDatabase()
        {
            return Database;
        }
        protected override string GetHead()
        {
            return DataAccesstor.GetHead()??base.GetHead();
        }
        protected override string GetPart(TIdentity identity)
        {
            return DataAccesstor.GetPart(identity);
        }
    }

}