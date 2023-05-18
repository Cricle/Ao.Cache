using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Ao.Cache.Benchmarks
{
    [JsonSerializable(typeof(Student))]
    internal partial class StudentJsonSerializerContext : JsonSerializerContext
    {

    }
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}