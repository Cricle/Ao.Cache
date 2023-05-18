using System.Runtime.CompilerServices;

namespace Ao.Cache
{
    public class CacheHelperCreator : ICacheHelperCreator
    {
        public CacheHelperCreator(IDataFinderFactory factory, ISyncDataFinderFactory syncFactory)
        {
            Factory = factory;
            SyncFactory = syncFactory;
        }

        public IDataFinderFactory Factory { get; }

        public ISyncDataFinderFactory SyncFactory { get; }

        public ICacheHelper<TReturn> GetHelper<TReturn>()
        {
            return CacheHelperStore<TReturn>.Get(Factory,SyncFactory);
        }
        static class CacheHelperStore<TReturn>
        {
            private static ICacheHelper<TReturn> instance;
            private static readonly object locker = new object();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ICacheHelper<TReturn> Get(IDataFinderFactory factory,ISyncDataFinderFactory syncFactory)
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new CacheHelper<TReturn>(factory,syncFactory);
                        }
                    }
                }
                return instance;
            }
        }
    }

}
