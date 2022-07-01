using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Ao.Cache.InLitedb
{
    public partial class LitedbCacheFinder<TIdentity, TEntry, TCollectionEntity>
    {
        public Expression<Func<TCollectionEntity,bool>> GetInvalidWhere()
        {
            return x => x.ExpirationTime != null && x.ExpirationTime <= DateTime.Now;
        }

        public ILiteQueryable<TCollectionEntity> LookupInvalidRows()
        {
            return Collection.Query()
                .Where(GetInvalidWhere());
        }

        public long GetCollectionSize()
        {
            return Collection.LongCount();
        }

        public List<TCollectionEntity> GetInvalidRows()
        {
            return LookupInvalidRows().ToList();
        }

        public int DeleteInvalidRows()
        {
            return Collection.DeleteMany(GetInvalidWhere());
        }
    }
}
