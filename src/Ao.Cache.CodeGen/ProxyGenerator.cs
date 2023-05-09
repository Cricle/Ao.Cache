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
        private static string GetVisibility(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.NotApplicable:
                    return string.Empty;
                case Accessibility.Private:
                    return "private";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "internal";
                case Accessibility.Public:
                default:
                    return "public";
            }
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
                var helperCreatorName = "helperCreator";
                var helperCreatorConstruct = $"{TypeConsts.ICacheHelperCreator} {helperCreatorName}";
                var pars = string.Empty;
                var @base = string.Empty;
                if (isClass)
                {
                    //Debugger.Launch();
                    var cs = ax as ClassDeclarationSyntax;
                    var constructor = cs.DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList();
                    var constructorModel = constructor.Select(x => syntaxContext.SemanticModel.GetDeclaredSymbol(x))
                        .Where(x => x.DeclaredAccessibility == Accessibility.ProtectedOrInternal || x.DeclaredAccessibility == Accessibility.Public).ToList();
                    var best = constructorModel.Where(x => x.GetAttributes().Any(y => y.ToString() == TypeConsts.CacheConstructorAttribute)).FirstOrDefault();
                    if (constructorModel.Count != 0)
                    {
                        if (best == null)
                        {
                            best = constructorModel[0];
                        }
                        if (best.Parameters.Length != 0)
                        {

                            pars = string.Join(",", best.Parameters.Select(p => $"{p.Type} {p.Name}"));
                            @base = $":base({string.Join(",", best.Parameters.Select(p => p.Name))})";
                            helperCreatorConstruct += ",";
                            var helperPar = best.Parameters.Where(x => x.Type.ToString() == TypeConsts.ICacheHelperCreator).FirstOrDefault();
                            if (helperPar != null)
                            {
                                helperCreatorName = helperPar.Name;
                                helperCreatorConstruct = string.Empty;
                            }
                        }
                    }
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
    {GetVisibility(declare.DeclaredAccessibility)} class {name}{proxyEndName} : {declare}
    {{        
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors| System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods)]
        private static readonly System.Type type = typeof({declare});
        {string.Join("\n        ", canProxys.Select(x=>$@" private static readonly System.Reflection.MethodInfo {GetMethodInfoName(methods.IndexOf(x),x,syntaxContext.SemanticModel)} = type.GetMethod(nameof({x.Identifier.ValueText}),{x.TypeParameterList?.Parameters.Count??0}, System.Reflection.BindingFlags.Public| System.Reflection.BindingFlags.Instance,null,new Type[] {{ {string.Join(",",x.ParameterList.Parameters.Select(y=>$"typeof({syntaxContext.SemanticModel.GetSymbolInfo(y.Type).Symbol})"))} }},null) ?? throw new ArgumentException($""{{type}} no method {{nameof({x.Identifier.ValueText})}}"");"))}
        {(isClass ? string.Empty : $"protected readonly {proxyType} _raw;")}
        private readonly {TypeConsts.ICacheHelperCreator} _helperCreator;

        public {name}{proxyEndName}({(isClass ? string.Empty : $"{proxyType} raw,")} {helperCreatorConstruct} {pars}){@base}
        {{
            {(isClass ? string.Empty : $"_raw = raw ?? throw new System.ArgumentNullException(nameof(raw));")}
            _helperCreator={helperCreatorName} ?? throw new System.ArgumentNullException(nameof({helperCreatorName}));
        }}
        {string.Join("\n", methods.Select(x => WriteMethod(context,x, syntaxContext.SemanticModel, isClass?"base":"_raw", "_helperCreator", canProxys.Contains(x), isClass, isProxyAll, methods.IndexOf(x))))}
    }}
}}
#pragma warning restore IDE1006
/// </auto-generated>
";
                var tree = CSharpSyntaxTree.ParseText(source);
                var root = tree.GetRoot();
                var formattedRoot = root.NormalizeWhitespace().ToFullString();
                //Debugger.Launch();
                context.AddSource($"{name}{proxyEndName}.g.cs", SourceText.From(formattedRoot, Encoding.UTF8));
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
            if (proxy)
            {
                if (!isTaskAsync && returnTypeSymbol is ITypeSymbol typeSymbol && !typeSymbol.IsReferenceType)
                {
                    if (typeSymbol.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ReturnMustRefOrNullable, Location.Create(method.SyntaxTree, method.Span)));
                    }
                }
                if (isTaskAsync && method.ReturnType is GenericNameSyntax genericNameSyntax &&
                    genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
                {
                    var arg = genericNameSyntax.TypeArgumentList.Arguments[0];
                    var symbol =(ITypeSymbol)model.GetSymbolInfo(arg).Symbol ;
                    if (!symbol.IsReferenceType && symbol.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ReturnMustRefOrNullable, Location.Create(method.SyntaxTree, method.Span)));
                    }
                }
            }
            else if(proxy&&isTaskAsync&& returnTypeSymbol is INamedTypeSymbol namedTypeSymbol&&namedTypeSymbol.IsGenericType)
            {
                var typeArg = namedTypeSymbol.TypeArguments[0];
                if (!typeArg.IsReferenceType&&typeArg.OriginalDefinition.SpecialType!= SpecialType.System_Nullable_T)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ReturnMustRefOrNullable, Location.Create(method.SyntaxTree, method.Span)));
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
                else if(method.ParameterList.Parameters.Count!=0)
                {
                    keyGen += $"string.Join(\",\",{string.Join(",",method.ParameterList.Parameters.Select(x=>x.Identifier.ValueText))});";
                }
                else
                {
                    keyGen += $"string.Empty;";
                }
                var s = $@"            var finder = {factoryName}.GetHelper<{actualRetType}>().{(isTaskAsync? "GetFinder": "GetFinderSync")}(type, {GetMethodInfoName(index,method,model)});
            {keyGen}
            var inCache = {(isTaskAsync?"await":string.Empty)} finder.{(isTaskAsync ? "FindInCacheAsync" : "FindInCache")}(key);
            if (inCache == null)
            {{
                var actual = {(isTaskAsync ? "await" : string.Empty)} {rawName}.{methodName}{typeParInline}({string.Join(",", method.ParameterList.Parameters.Select(x =>x.Identifier.ValueText))});
                if (actual != null)
                {{
                    {(isTaskAsync ? "await" : string.Empty)} finder.{(isTaskAsync ? "SetInCacheAsync" : "SetInCache")}(key, actual);
                }}
                return actual;
            }}

            return inCache;
";
                head += s+ "\n";
            }
            else if(isClass)
            {
                return string.Empty;
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
