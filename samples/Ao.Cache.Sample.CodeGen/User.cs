using LiteDB;

namespace Ao.Cache.Sample.CodeGen
{
    public record class User
    {
        public ObjectId? Id { get; set; }

        public string? Name { get; set; }

        public int Number { get; set; }
    }
}