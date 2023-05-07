using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace Ao.Cache.CodeGen
{
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
        protected bool GetProxyAll(AttributeData attributeData)
        {
            var res = GetAttributeHelper.GetValue<string>(attributeData, TypeConsts.CacheProxyProxyAllAttribute);
            if (string.IsNullOrEmpty(res))
            {
                return true;
            }
            return bool.Parse(res);
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
        protected bool GetInline(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<bool>(attributeData, TypeConsts.CacheProxyMethodInlineAttribute);
        }
        protected bool GetNoProxy(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<bool>(attributeData, TypeConsts.CacheProxyMethodNoProxyAttribute);
        }
        protected string GetCacheTime(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<string>(attributeData, TypeConsts.CacheProxyMethodCacheTimeAttribute);
        }
        protected string GetHead(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<string>(attributeData, TypeConsts.Head);
        }
        protected bool GetHeadAbsolute(AttributeData attributeData)
        {
            return GetAttributeHelper.GetValue<bool>(attributeData, TypeConsts.HeadAbsolute);
        }
        protected void ExecuteProxy(SourceProductionContext context, SyntaxNode ax, GeneratorSyntaxContext syntaxContext, AttributeData attributeData)
        {
            var isProxyAll = GetProxyAll(attributeData);
            var canProxys = new HashSet<MethodDeclarationSyntax>();
            var declare = syntaxContext.SemanticModel.GetDeclaredSymbol(ax);
            var isInterface = ax is InterfaceDeclarationSyntax;
            var isClass = ax is ClassDeclarationSyntax;
            var methods =((isInterface ? ((InterfaceDeclarationSyntax)ax).Members : ((ClassDeclarationSyntax)ax).Members).OfType<MethodDeclarationSyntax>()).ToList();
            foreach (var item in methods)
            {
                var methodDecalre = syntaxContext.SemanticModel.GetDeclaredSymbol(item);
                if (!isProxyAll)
                {
                    if (!methodDecalre.GetAttributes().Any(attr => attr.AttributeClass?.ToString().Equals(TypeConsts.CacheProxyMethodAttribute) ?? false))
                    {
                        continue;
                    }
                }
                var symbol = syntaxContext.SemanticModel.GetSymbolInfo(item.ReturnType);
                var symbolName = symbol.Symbol?.ToString();
                var ok = symbolName != "void" && symbolName != TypeConsts.TaskTypeName && symbolName != TypeConsts.ValueTaskTypeName;
                if (ok)
                {
                    canProxys.Add(item);
                }
            }
            var proxyType = GetProxyType(attributeData);
            if (string.IsNullOrEmpty(proxyType))
            {
                if (isInterface)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.InterfaceProxyMustGivenProxyType, Location.Create(ax.SyntaxTree, ax.Span)));
                    return;
                }
                proxyType = declare.ToString();
            }
            var proxyEndName = GetEndName(attributeData) ?? TypeConsts.ProxyDefaultEndName;
            var @namespace = GetNameSpace(attributeData) ?? TypeConsts.DefaultNameSpace;
            var name = proxyType.Split('.').Last();
            try
            {
                var pars = string.Empty;
                var @base = string.Empty;
                if (isClass)
                {
                    //Debugger.Launch();
                    var cs = ax as ClassDeclarationSyntax;
                    var constructor=cs.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList();
                    var constructorModel = constructor.Select(x => syntaxContext.SemanticModel.GetDeclaredSymbol(x))
                        .Where(x=>x.DeclaredAccessibility== Accessibility.ProtectedOrInternal||x.DeclaredAccessibility== Accessibility.Public).ToList();
                    var best = constructorModel.Where(x => x.GetAttributes().Any(y=> y.ToString() == TypeConsts.CacheConstructorAttribute)).FirstOrDefault();
                    if (best == null)
                    {
                        best = constructorModel[0];
                    }
                    pars = " ,"+string.Join(",", best.Parameters.Select(p => $"{p.Type} {p.Name}"));
                    @base = $":base({string.Join(",", best.Parameters.Select(p => p.Name))})";
                }
                var source = $@"
/// <auto-generated>
#pragma warning disable IDE1006

using System;
using Ao.Cache;
using System.Runtime.CompilerServices;

namespace {@namespace}
{{
    [System.Runtime.CompilerServices.CompilerGenerated]
    {InternalData.GeneratedCode}
    [System.Diagnostics.DebuggerStepThrough]
    [{(isClass?TypeConsts.CacheProxyByClassAttribute:TypeConsts.CacheProxyByInterfaceAttribute)}({TypeConsts.CacheProxyByProxyTypeAttribute}=typeof({declare}))]
    public class {name}{proxyEndName} : {declare}
    {{
        private static readonly System.Type type = typeof({declare});
        {string.Join("\n        ", canProxys.Select(x=>$@" private static readonly System.Reflection.MethodInfo {GetMethodInfoName(methods.IndexOf(x),x,syntaxContext.SemanticModel)} = type.GetMethod(nameof({x.Identifier.ValueText}),{x.TypeParameterList?.Parameters.Count??0}, System.Reflection.BindingFlags.Public| System.Reflection.BindingFlags.Instance,null,new Type[] {{ {string.Join(",",x.ParameterList.Parameters.Select(y=>$"typeof({syntaxContext.SemanticModel.GetSymbolInfo(y.Type).Symbol})"))} }},null) ?? throw new ArgumentException($""{{type}} no method {{nameof({x.Identifier.ValueText})}}"");"))}
        {(isClass ? string.Empty : $"protected readonly {proxyType} _raw;")}
        protected readonly ICacheHelperCreator _helperCreator;

        public {name}{proxyEndName}({(isClass ? string.Empty : $"{proxyType} raw,")} ICacheHelperCreator helperCreator{pars}){@base}
        {{
            {(isClass ? string.Empty : $"_raw = raw ?? throw new System.ArgumentNullException(nameof(raw));")}
            _helperCreator=helperCreator ?? throw new System.ArgumentNullException(nameof(helperCreator));
        }}
        {string.Join("\n", methods.Select(x => WriteMethod(context,x, syntaxContext.SemanticModel, isClass?"base":"_raw", "_helperCreator", canProxys.Contains(x), isClass, isProxyAll, methods.IndexOf(x))))}
    }}
}}
#pragma warning restore IDE1006
/// </auto-generated>
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
        private string GetMethodInfoName(int index,MethodDeclarationSyntax syntax,SemanticModel model)
        {
            return $"{syntax.Identifier.ValueText}_{index}";
        }
        private string WriteMethod(SourceProductionContext context,MethodDeclarationSyntax method, SemanticModel model, string rawName, string factoryName, bool proxy, bool isClass,bool proxyAll,int index)
        {
            var methodtree = model.GetDeclaredSymbol(method);
            if (isClass && !methodtree.IsVirtual)
            {
                return string.Empty;
            }
            var attr = GetAttributeHelper.GetAttribute(method, model, TypeConsts.CacheProxyMethodAttribute);
            if (!proxyAll && attr == null)
            {
                return string.Empty;
            }
            var returnTypeSymbol = model.GetSymbolInfo(method.ReturnType).Symbol;
            var returnType = returnTypeSymbol.ToString();
            var actualRetType = returnType;
            var isTaskAsync = returnType.StartsWith(TypeConsts.TaskTypeName) || returnType.StartsWith(TypeConsts.ValueTaskTypeName);
            if (isTaskAsync)
            {
                actualRetType = returnType.Substring(returnType.IndexOf('<') + 1, returnType.Length - returnType.IndexOf('<') - 2);
            }
            if (proxy &&!isTaskAsync&& returnTypeSymbol is ITypeSymbol typeSymbol&&!typeSymbol.IsReferenceType)
            {
                if (typeSymbol.OriginalDefinition.SpecialType!= SpecialType.System_Nullable_T)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ReturnMustRefOrNullable,Location.Create(method.SyntaxTree,method.Span)));
                }
            }

            var methodName = method.Identifier.ValueText;

            var parameters = string.Join(", ", method.ParameterList.Parameters
    .Select(p => $"{model.GetSymbolInfo(p.Type).Symbol} {p.Identifier.ValueText}"));

            var typeParameters = string.Join(", ", method.TypeParameterList?.Parameters
                .Select(p => p.Identifier.ValueText) ?? Enumerable.Empty<string>());
            var typeParInline = string.IsNullOrEmpty(typeParameters) ? string.Empty : $"<{typeParameters}>";
            var inline = GetInline(attr);
            var noProxy = GetNoProxy(attr);
            if (!proxyAll && noProxy)
            {
                if (isClass)
                {
                    return string.Empty;
                }
                proxy = false;
            }
            var head = $@"
        {((attr==null||inline) ? "[MethodImpl(MethodImplOptions.AggressiveInlining)]":string.Empty)}
        public {(isClass ? "override " : string.Empty)}{((isTaskAsync&&proxy) ? "async " : string.Empty)}{returnType} {methodName}{(string.IsNullOrEmpty(typeParameters) ? string.Empty : $"<{typeParameters}>")}({parameters})
        {{
";
            if (proxy)
            {
                var keyGen = "var key = ";
                if (method.ParameterList.Parameters.Count==1)
                {
                    var par= method.ParameterList.Parameters[0];
                    keyGen += par.Identifier.ValueText;
                    var parSymbol=(ITypeSymbol)model.GetSymbolInfo(par.Type).Symbol;
                    if (parSymbol.IsReferenceType)
                    {
                        keyGen += "?";
                    }
                    keyGen += ".ToString();";
                }
                else
                {
                    keyGen += $"string.Join(\",\",{string.Join(",",method.ParameterList.Parameters.Select(x=>x.Identifier.ValueText))});";
                }
                var s = $@"            var finder = {factoryName}.GetHelper<{actualRetType}>().GetFinder(type, {GetMethodInfoName(index,method,model)});
            {keyGen}
            var inCache = {(isTaskAsync?"await":string.Empty)} finder.FindInCacheAsync(key){(isTaskAsync ? string.Empty: ".GetAwaiter().GetResult()")};
            if (inCache == null)
            {{
                var actual = {(isTaskAsync ? "await" : string.Empty)} {rawName}.{methodName}{typeParInline}({string.Join(",", method.ParameterList.Parameters.Select(x =>x.Identifier.ValueText))});
                if (actual != null)
                {{
                    {(isTaskAsync ? "await" : string.Empty)} finder.SetInCacheAsync(key, actual){(isTaskAsync ? string.Empty : ".GetAwaiter().GetResult()")};
                }}
                return actual;
            }}

            return inCache;
";
                head += s+ "\n";
            }
            else
            {
                head += $@"
            {(returnType == "void" ? string.Empty : "return")} {(isClass ? "base" : rawName)}.{methodName}({string.Join(", ", method.ParameterList.Parameters.Select(x => x.Identifier.ValueText))});
";
            }
            head += "        }\n";
            return head;
        }
    }
    internal class AoCacheException : Exception
    {
        public Diagnostic Error { get; set; }
    }
}
