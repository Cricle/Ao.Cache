namespace Ao.Cache
{
    internal class Finders<TReturn>
    {
        public readonly IDataFinder<string, TReturn> Finder;

        public readonly ISyncDataFinder<string,TReturn> SyncFinder;

        public Finders(IDataFinder<string, TReturn> finder, ISyncDataFinder<string, TReturn> syncFinder)
        {
            Finder = finder;
            SyncFinder = syncFinder;
        }
    }

}
