using System.Collections.Generic;

namespace Ao.Cache.RWSeparation
{
    public interface IStatusDistribution<TStatus>
    {
        TStatus GetNext(TStatus status);
        TStatus GetPrevious(TStatus status);

        TStatus Fist { get; }
        TStatus Last { get; }

        IEnumerable<TStatus> AllStatus { get; }

        bool InStatus(TStatus status);
    }
}
