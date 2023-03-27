using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace Ao.Cache.CodeGen
{
    internal static class GetAttributeHelper
    {
        public static T GetValue<T>(AttributeData attributeData,string name)
        {
            var f = attributeData.NamedArguments.FirstOrDefault(x => x.Key == name);
            if (typeof(T)==typeof(string))
            {
                return f.Key == null ? default : (T)(object)f.Value.Value?.ToString();
            }
            return f.Key == null ? default : (T)f.Value.Value;
        }
        public static AttributeData GetCacheProxyAttribute(SyntaxNode node, SemanticModel model,string attributeName)
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
            var cacheProxyAttr = GetAttributeHelper.GetCacheProxyAttribute(ax, syntaxContext.SemanticModel,TypeConsts.CacheProxyAttribute);
            if (ax is InterfaceDeclarationSyntax ids)
            {
                ExecuteInterface(context, ids, syntaxContext, cacheProxyAttr);
            }
            else if (ax is ClassDeclarationSyntax cds)
            {
                ExecuteClass(context, cds, syntaxContext, cacheProxyAttr);
            }
        }
        protected string GetProxyType(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<string>(attributeData, TypeConsts.CacheProxyAttributeProxyTypeName);
        }
        protected string GetEndName(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<string>(attributeData, TypeConsts.CacheProxyAttributeEndNameName);
        }
        protected void ExecuteInterface(SourceProductionContext context, InterfaceDeclarationSyntax ax, GeneratorSyntaxContext syntaxContext, AttributeData attributeData)
        {
            var canProxys = new HashSet<MethodDeclarationSyntax>();
            var declare = syntaxContext.SemanticModel.GetDeclaredSymbol(ax);
            var methods = ax.Members.OfType<MethodDeclarationSyntax>();
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
            if (no1ArgMethod!=null)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ProxyParaNo1, Location.Create(no1ArgMethod.SyntaxTree, no1ArgMethod.Span)));
                return;
            }
            var proxyType = GetProxyType(attributeData);
            var proxyEndName = GetEndName(attributeData) ?? TypeConsts.ProxyDefaultEndName;
            var source = $@"
using System;
using Ao.Cache;

public class {ax.Identifier}{proxyEndName} : {declare}
{{
    protected readonly {proxyType} _raw;
    protected readonly IDataFinderFactory _factory;

    public {ax.Identifier}{proxyEndName}({proxyType} raw, IDataFinderFactory factory)
    {{
        _raw=raw;
        _factory=factory;
    }}
    {string.Join("\n", methods.Select(x => WriteMethod(x, syntaxContext.SemanticModel, "_raw", "_factory",canProxys.Contains(x))))}

    {string.Join("\n", canProxys.Select(x => MakeGenerate(x, syntaxContext.SemanticModel, proxyType, "_raw")))}
}}
";
            context.AddSource($"{ax.Identifier}Proxy.g.cs",source);
        }
        private string MakeGenerate(MethodDeclarationSyntax x,SemanticModel model,string proxyType,string rawName)
        {
            var genericeTypeStr=string.Empty;
            if (x.TypeParameterList?.Parameters != null)
            {
                var genericeTypes = string.Join(", ", x.TypeParameterList.Parameters.Select(w => w.Identifier.ValueText));
                if (!string.IsNullOrEmpty(genericeTypes))
                {
                    genericeTypeStr = "<" + genericeTypes + ">";
                }
            }
            var ret = model.GetSymbolInfo(x.ReturnType).Symbol.ToString();
            var isTaskAsync = ret.StartsWith(TypeConsts.TaskTypeName);
            var isValueTaskAsync = ret.StartsWith(TypeConsts.ValueTaskTypeName);
            string body;
            var actualRetType = ret;
            var methodRetType = ret;
            if (isTaskAsync||isValueTaskAsync)
            {
                actualRetType = ret.Substring(ret.IndexOf('<') + 1, ret.Length - ret.IndexOf('<') - 2);
            }
            if (isTaskAsync)
            {
                body = $"return {rawName}.{x.Identifier.ValueText}{genericeTypeStr}(identity);";
            }
            else if (isValueTaskAsync)
            {
                body = $"return await {rawName}.{x.Identifier.ValueText}{genericeTypeStr}(identity);";
                methodRetType = TypeConsts.TaskTypeName + "<" + actualRetType + ">";
            }
            else
            {
                body = $"return Task.FromResult<{ret}>({rawName}.{x.Identifier.ValueText}{genericeTypeStr}(identity));";
                methodRetType = TypeConsts.TaskTypeName + "<" + actualRetType + ">";
            }
            var arg1 = model.GetSymbolInfo(x.ParameterList.Parameters[0].Type).Symbol;
            var name = x.Identifier.ValueText + "DataAccesstor";
            return $@"
private readonly struct {name}{genericeTypeStr} : IDataAccesstor<{arg1},{actualRetType}>
{{
    private readonly {proxyType} {rawName};
    
    public {name}({proxyType} {rawName})
    {{
        this.{rawName} = {rawName};
    }}

    public {((isValueTaskAsync) ? "async": string.Empty)} {methodRetType} FindAsync({arg1} identity)
    {{
        {body}
    }}
}}
";
        }
        private string WriteMethod(MethodDeclarationSyntax method,SemanticModel model,string rawName,string factoryName, bool proxy)
        {
            var returnType=model.GetSymbolInfo(method.ReturnType).Symbol.ToString();
            var isTaskAsync = returnType.StartsWith(TypeConsts.TaskTypeName) || returnType.StartsWith(TypeConsts.ValueTaskTypeName);
            var isValueTaskAsync = returnType.StartsWith(TypeConsts.ValueTaskTypeName);

            var methodName = method.Identifier.ValueText;

            var parameters = string.Join(", ", method.ParameterList.Parameters
                .Select(p => $"{model.GetSymbolInfo(p.Type).Symbol} {p.Identifier.ValueText}"));

            var typeParameters = string.Join(", ", method.TypeParameterList?.Parameters
                .Select(p => p.Identifier.ValueText) ?? Enumerable.Empty<string>());
                var accesstorName = methodName + "DataAccesstor";
            var head = $@"
public {(isValueTaskAsync?"async":string.Empty)} {returnType} {methodName}{(string.IsNullOrEmpty(typeParameters) ? string.Empty : $"<{typeParameters}>")}({parameters})
{{
";
            if (proxy)
            {

                head+=$@"
        var finder = {factoryName}.Create(new {accesstorName}{(string.IsNullOrEmpty(typeParameters)?string.Empty:$"<{typeParameters}>")}({rawName}));
        return {((isValueTaskAsync ? "await" : string.Empty))} finder.FindAsync({string.Join(", ", method.ParameterList.Parameters.Select(x => x.Identifier.ValueText))}){(isTaskAsync?string.Empty: ".GetAwaiter().GetResult()")};
";
            }
            else
            {
                head += $@"
        {(returnType=="void"?string.Empty:"return")} {rawName}.{methodName}({string.Join(", ", method.ParameterList.Parameters.Select(x => x.Identifier.ValueText))});
";
            }
            head += "}\n";
            return head;
        }
        protected void ExecuteClass(SourceProductionContext context, ClassDeclarationSyntax ax, GeneratorSyntaxContext syntaxContext, AttributeData attributeData)
        {

        }
    }
}
