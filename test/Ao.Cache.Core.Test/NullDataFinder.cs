using System;
using System.Threading.Tasks;

namespace Ao.Cache.Core.Test
{
    internal class NullDataFinder : DataFinderBase<int, string>
    {
        public override Task<bool> DeleteAsync(int entity)
        {
            return Task.FromResult(true);
        }

        public override Task<bool> ExistsAsync(int identity)
        {
            return Task.FromResult(identity != 0);
        }

        public override Task<bool> RenewalAsync(int identity, TimeSpan? time)
        {
            return Task.FromResult(true);
        }

        protected override Task<string> CoreFindInCacheAsync(string key, int identity)
        {
            if (identity > 10)
            {
                return Task.FromResult<string>(null);
            }
            return Task.FromResult(identity.ToString());
        }

        protected override Task<string> OnFindInDbAsync(int identity)
        {
            return Task.FromResult((identity % 10).ToString());
        }

        protected override Task<bool> WriteCacheAsync(string key, int identity, string entity, TimeSpan? caheTime)
        {
            return Task.FromResult(true);
        }
    }
}
