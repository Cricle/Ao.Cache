using Microsoft.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Ao.Cache.CodeGen
{
    internal static class GetAttributeHelper
    {
        public static T GetValue<T>(AttributeData attributeData, string name)
        {
            if (attributeData == null)
            {
                return default;
            }
            var f = attributeData.NamedArguments.FirstOrDefault(x => x.Key == name);
            if (typeof(T) == typeof(string))
            {
                return f.Key == null ? default : (T)(object)f.Value.Value?.ToString();
            }
            return f.Key == null ? default : (T)f.Value.Value;
        }
        public static AttributeData GetAttribute(SyntaxNode node, SemanticModel model, string attributeName)
        {
            var decalre = model.GetDeclaredSymbol(node);
            var attr = decalre.GetAttributes().FirstOrDefault(x => x.AttributeClass?.ToString().Equals(attributeName) ?? false);
            return attr;
        }
    }
}
