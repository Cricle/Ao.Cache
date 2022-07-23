using System;
using System.Collections.Generic;
using System.Text;

namespace Ao.Cache
{
    public interface IWithDataAccesstorFinder<TIdentity, TEntry>
    {
        IDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }
    }
    public interface IWithBatchDataAccesstorFinder<TIdentity, TEntry>
    {
        IBatchDataAccesstor<TIdentity, TEntry> DataAccesstor { get; }
    }
}
