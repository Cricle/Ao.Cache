using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Runtime.InteropServices;

namespace Ao.Cache.CodeGen
{
    internal static class GetAttributeHelper
    {
        public static T GetValue<T>(AttributeData attributeData, string name)
        {
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
            var attr = decalre.GetAttributes().First(x => x.AttributeClass?.ToString().Equals(attributeName) ?? false);
            return attr;
        }
    }
    [Generator]
    public class ProxyGenerator : IIncrementalGenerator
    {
        private IncrementalGeneratorInitializationContext context;

        protected IncrementalGeneratorInitializationContext Context => context;

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            this.context = context;
            var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
               (sn, tk) => (sn is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0) || (sn is InterfaceDeclarationSyntax ids && ids.AttributeLists.Count > 0),
               (ctx, tk) =>
               {
                   var cq = ctx.SemanticModel;
                   var isDataAccesstor = cq.GetDeclaredSymbol(ctx.Node).GetAttributes().Any(x => TypeConsts.CacheProxyAttribute.Equals(x.AttributeClass?.ToString()));
                   if (isDataAccesstor)
                   {
                       return new Tuple<SyntaxNode, GeneratorSyntaxContext>(ctx.Node, ctx);
                   }
                   return null;
               }).Where(x => x != null);
            context.RegisterSourceOutput(syntaxProvider, (ctx, source) => Execute(ctx, source.Item1, source.Item2));
        }
        protected void Execute(SourceProductionContext context, SyntaxNode ax, GeneratorSyntaxContext syntaxContext)
        {
            var cacheProxyAttr = GetAttributeHelper.GetAttribute(ax, syntaxContext.SemanticModel, TypeConsts.CacheProxyAttribute);
            ExecuteProxy(context, ax, syntaxContext, cacheProxyAttr);
        }
        protected string GetProxyType(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<string>(attributeData, TypeConsts.CacheProxyAttributeProxyTypeName);
        }
        protected string GetEndName(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<string>(attributeData, TypeConsts.CacheProxyAttributeEndNameName);
        }
        protected string GetNameSpace(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<string>(attributeData, TypeConsts.CacheProxyAttributeNameSpaceName);
        }
        protected string GetRenewal(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<string>(attributeData, TypeConsts.CacheProxyMethodRenewalAttribute);
        }
        protected string GetCacheTime(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<string>(attributeData, TypeConsts.CacheProxyMethodCacheTimeAttribute);
        }
        protected void ExecuteProxy(SourceProductionContext context, SyntaxNode ax, GeneratorSyntaxContext syntaxContext, AttributeData attributeData)
        {
            var canProxys = new HashSet<MethodDeclarationSyntax>();
            var declare = syntaxContext.SemanticModel.GetDeclaredSymbol(ax);
            var isInterface = ax is InterfaceDeclarationSyntax;
            var isClass = ax is ClassDeclarationSyntax;
            var methods = (isInterface ? ((InterfaceDeclarationSyntax)ax).Members : ((ClassDeclarationSyntax)ax).Members).OfType<MethodDeclarationSyntax>();
            foreach (var item in methods)
            {
                var methodDecalre = syntaxContext.SemanticModel.GetDeclaredSymbol(item);
                if (!methodDecalre.GetAttributes().Any(attr => attr.AttributeClass?.ToString().Equals(TypeConsts.CacheProxyMethodAttribute) ?? false))
                {
                    continue;
                }
                var symbol = syntaxContext.SemanticModel.GetSymbolInfo(item.ReturnType);
                var symbolName = symbol.Symbol?.ToString();
                var ok = symbolName != "void" && symbolName != TypeConsts.TaskTypeName && symbolName != TypeConsts.ValueTaskTypeName;
                canProxys.Add(item);
            }
            //Check must input 1 arg
            var no1ArgMethod = canProxys.FirstOrDefault(x => x.ParameterList.Parameters.Count != 1);
            if (no1ArgMethod != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ProxyParaNo1, Location.Create(no1ArgMethod.SyntaxTree, no1ArgMethod.Span)));
                return;
            }
            var proxyType = GetProxyType(attributeData);
            if (string.IsNullOrEmpty(proxyType))
            {
                if (isInterface)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.InterfaceProxyMustGivenProxyType, Location.Create(no1ArgMethod.SyntaxTree, no1ArgMethod.Span)));
                    return;
                }
                proxyType = declare.ToString();
            }
            var proxyEndName = GetEndName(attributeData) ?? TypeConsts.ProxyDefaultEndName;
            var @namespace = GetNameSpace(attributeData) ?? TypeConsts.DefaultNameSpace;
            var name = proxyType.Split('.').Last();
            try
            {
                var source = $@"
/// <auto-generated>

using System;
using Ao.Cache;
using System.Runtime.CompilerServices;

namespace {@namespace}
{{
    {InternalData.GeneratedCode}
    [ System.Diagnostics.DebuggerStepThrough]
    public class {name}{proxyEndName} : {declare}
    {{
        {(isClass ? string.Empty : $"protected readonly {proxyType} _raw;")}
        protected readonly IDataFinderFactory _factory;

        public {name}{proxyEndName}({(isClass ? string.Empty : $"{proxyType} raw,")} IDataFinderFactory factory)
        {{
            {(isClass ? string.Empty : $"_raw=raw ?? throw new System.ArgumentNullException(nameof(raw));")}
            _factory=factory ?? throw new System.ArgumentNullException(nameof(factory));
        }}
        {string.Join("\n", methods.Select(x => WriteMethod(x, syntaxContext.SemanticModel, "_raw", "_factory", canProxys.Contains(x), isClass)))}
    }}
}}
";
                context.AddSource($"{name}{proxyEndName}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
            catch (AoCacheException ex)
            {
                if (ex.Error != null)
                {
                    context.ReportDiagnostic(ex.Error);
                }
            }
        }
        private string WriteMethod(MethodDeclarationSyntax method, SemanticModel model, string rawName, string factoryName, bool proxy, bool isClass)
        {
            var methodtree = model.GetDeclaredSymbol(method);
            if (isClass && !methodtree.IsVirtual)
            {
                return string.Empty;
            }
            var returnType = model.GetSymbolInfo(method.ReturnType).Symbol.ToString();
            var isTaskAsync = returnType.StartsWith(TypeConsts.TaskTypeName) || returnType.StartsWith(TypeConsts.ValueTaskTypeName);
            var isValueTaskAsync = returnType.StartsWith(TypeConsts.ValueTaskTypeName);

            var methodName = method.Identifier.ValueText;

            var parameters = string.Join(", ", method.ParameterList.Parameters
                .Select(p => $"{model.GetSymbolInfo(p.Type).Symbol} {p.Identifier.ValueText}"));

            var typeParameters = string.Join(", ", method.TypeParameterList?.Parameters
                .Select(p => p.Identifier.ValueText) ?? Enumerable.Empty<string>());
            var accesstorName = methodName + "DataAccesstor";
            var head = $@"
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public {(isClass ? "override " : string.Empty)}{(isValueTaskAsync ? "async" : string.Empty)} {returnType} {methodName}{(string.IsNullOrEmpty(typeParameters) ? string.Empty : $"<{typeParameters}>")}({parameters})
    {{
";
            if (proxy)
            {
                var attr = GetAttributeHelper.GetAttribute(method, model, TypeConsts.CacheProxyMethodAttribute);
                var cacheTime = GetCacheTime(attr);
                var renewal = GetRenewal(attr);
                var actualRetType = returnType;
                if (isTaskAsync || isValueTaskAsync)
                {
                    actualRetType = returnType.Substring(returnType.IndexOf('<') + 1, returnType.Length - returnType.IndexOf('<') - 2);
                }
                var arg1 = model.GetSymbolInfo(method.ParameterList.Parameters[0].Type).Symbol;
                var body = $@"identity=>{(isClass ? "base" : rawName)}.{methodName}{(string.IsNullOrEmpty(typeParameters) ? string.Empty : $"<{typeParameters}>")}(identity)";
                if (isTaskAsync)
                {
                    body = $@"{(isClass ? "base" : rawName)}.{methodName}{(string.IsNullOrEmpty(typeParameters) ? string.Empty : $"<{typeParameters}>")}";
                }
                head += $@"
        var finder = {factoryName}.Create(new DelegateDataAccesstor<{arg1},{actualRetType}>({body}));
";
                if (!string.IsNullOrEmpty(cacheTime))
                {
                    if (!TimeSpan.TryParse(cacheTime, out var tp))
                    {
                        throw new AoCacheException
                        {
                            Error = Diagnostic.Create(DiagnosticDescriptors.CacheTimeCanNotConvert, Location.Create(method.SyntaxTree, method.Span))
                        };
                    }
                    head += $@"
        finder.Options.WithCacheTime(new TimeSpan({tp.Ticks}L));
";
                }
                var renewalWrite = !string.IsNullOrEmpty(renewal) && renewal.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
                head += $@"
        finder.Options.WithRenew({renewalWrite.ToString().ToLower()});
";
                head += "\n" + $@"    return {((isValueTaskAsync ? "await" : string.Empty))} finder.FindAsync({string.Join(", ", method.ParameterList.Parameters.Select(x => x.Identifier.ValueText))}){(isTaskAsync ? string.Empty : ".GetAwaiter().GetResult()")};" + "\n";
            }
            else
            {
                head += $@"
        {(returnType == "void" ? string.Empty : "return")} {(isClass ? "base" : rawName)}.{methodName}({string.Join(", ", method.ParameterList.Parameters.Select(x => x.Identifier.ValueText))});
";
            }
            head += "   }\n";
            return head;
        }
    }
    internal class AoCacheException : Exception
    {
        public Diagnostic Error { get; set; }
    }
}
