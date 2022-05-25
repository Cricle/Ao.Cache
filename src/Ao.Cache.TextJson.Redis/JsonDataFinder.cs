using System;
using System.Text.Json;

namespace Ao.Cache.TextJson.Redis
{
    public abstract class JsonDataFinder<TIdentity, TEntry> : DataFinderBase<TIdentity, TEntry>
    {
        private static readonly JsonSerializerOptions defaultOptions = new JsonSerializerOptions();

        protected virtual byte[] ToBytes(TEntry entry)
        {
            var options = GetOptions();
            return JsonSerializer.SerializeToUtf8Bytes(entry, options);
        }
        protected virtual TEntry ToEntry(in ReadOnlyMemory<byte> bytes)
        {
            var options = GetOptions();
            return JsonSerializer.Deserialize<TEntry>(bytes.Span, options);
        }
        public override string GetHead()
        {
            return "Json." + base.GetHead();
        }
        protected virtual JsonSerializerOptions GetOptions()
        {
            return defaultOptions;
        }
    }

}
