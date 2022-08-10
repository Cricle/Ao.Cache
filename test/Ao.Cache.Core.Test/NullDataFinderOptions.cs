using System;

namespace Ao.Cache.Core.Test
{
    internal class ValueDataFinderOptions<TIdentity, TEntity> : IDataFinderOptions<TIdentity, TEntity>
    {
        public bool CanRenewalValue { get; set; }
        public TimeSpan? GetGetCacheTimeValue { get; set; }
        public string GetEntryKeyValue { get; set; }
        public string GetHeadValue { get; set; }
        public string GetPartValue { get; set; }

        public bool CanRenewal(TIdentity identity)
        {
            return CanRenewalValue;
        }

        public TimeSpan? GetCacheTime(TIdentity identity)
        {
            return GetGetCacheTimeValue;
        }

        public string GetEntryKey(TIdentity identity)
        {
            return GetEntryKeyValue;
        }

        public string GetHead()
        {
            return GetHeadValue;
        }

        public string GetPart(TIdentity identity)
        {
            return GetPartValue;
        }
    }
    internal class NullDataFinderOptions<TIdentity, TEntity> : IDataFinderOptions<TIdentity, TEntity>
    {
        public bool CanRenewal(TIdentity identity)
        {
            return false;
        }

        public TimeSpan? GetCacheTime(TIdentity identity)
        {
            return null;
        }

        public string GetEntryKey(TIdentity identity)
        {
            return null;
        }

        public string GetHead()
        {
            return null;
        }

        public string GetPart(TIdentity identity)
        {
            return null;
        }
    }
}
