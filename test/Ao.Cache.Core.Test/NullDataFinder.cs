using System;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    internal class NullDataFinder : DataFinderBase<int, string>
    {
        public override bool Delete(int identity)
        {
            return true;
        }

        public override Task<bool> DeleteAsync(int entity)
        {
            return Task.FromResult(true);
        }

        public override bool Exists(int identity)
        {
            return true;
        }

        public override Task<bool> ExistsAsync(int identity)
        {
            return Task.FromResult(identity != 0);
        }

        public override bool Renewal(int identity, TimeSpan? time)
        {
            return true;
        }

        public override Task<bool> RenewalAsync(int identity, TimeSpan? time)
        {
            return Task.FromResult(true);
        }

        protected override string CoreFindInCache(string key, int identity)
        {
            return key;
        }

        protected override Task<string> CoreFindInCacheAsync(string key, int identity)
        {
            if (identity > 10)
            {
                return Task.FromResult<string>(null);
            }
            return Task.FromResult(identity.ToString());
        }

        protected override bool SetInCache(string key, int identity, string entity, TimeSpan? caheTime)
        {
            return true;
        }

        protected override Task<bool> SetInCacheAsync(string key, int identity, string entity, TimeSpan? caheTime)
        {
            return Task.FromResult(true);
        }
    }
}
