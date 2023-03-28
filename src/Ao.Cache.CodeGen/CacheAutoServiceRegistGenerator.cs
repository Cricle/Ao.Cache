using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Ao.Cache.CodeGen
{
    [Generator]
    public class CacheAutoServiceRegistGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
               (sn, tk) => sn is AttributeSyntax @as&&(@as.Name.ToString()== TypeConsts.EnableCacheAutoServiceRegistShort|| @as.Name.ToString() == TypeConsts.EnableCacheAutoServiceRegistAttributeShort),
               (ctx, tk) => ctx.Node);
            context.RegisterSourceOutput(syntaxProvider, Execute);
        }
        protected void Execute(SourceProductionContext context,SyntaxNode node)
        {
            var sources = $@"
using Ao.Cache;
using Ao.Cache.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

internal static class EnableProxyServiceExtensions
{{
    public static IServiceCollection AddProxy<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {{
        return AddProxy(services, typeof(T), lifetime);
    }}
    public static IServiceCollection AddProxy(this IServiceCollection services, Type targetType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {{
        var isClass = targetType.IsClass;
        var x = targetType.Assembly.GetExportedTypes()
            .Where(x =>
            {{
                var interfaceAttr = x.GetCustomAttribute<CacheProxyByAttribute>();
                return interfaceAttr != null && interfaceAttr.ProxyType == targetType;
            }}).FirstOrDefault();
        if (x == null)
        {{
            throw new ArgumentException($""Type {{targetType}} has not any CacheProxyByAttribute, ensure it is auto-gen, and version is right"");
        }}
        if (x != null)
        {{
            if (isClass)
            {{
                services.Add(new ServiceDescriptor(targetType, x, lifetime));
            }}
            else
            {{
                var attr = targetType.GetCustomAttribute<CacheProxyAttribute>();
                if (attr == null)
                {{
                    throw new ArgumentException($""Type {{targetType}} does not has attribute {{typeof(CacheProxyAttribute)}}"");
                }}
                var proxy = x.GetCustomAttribute<CacheProxyByAttribute>();
                services.Add(new ServiceDescriptor(attr.ProxyType, attr.ProxyType, lifetime));
                services.Add(new ServiceDescriptor(targetType, provider =>
                {{
                    return ActivatorUtilities.CreateInstance(provider, x,
                        ActivatorUtilities.CreateInstance(provider, attr.ProxyType, new object[0]),
                        provider.GetRequiredService<IDataFinderFactory>());
                }}, lifetime));
            }}
        }}
        return services;
    }}
}}
";
            context.AddSource("EnableProxyServiceExtensions.g.cs", SourceText.From(sources,Encoding.UTF8));
        }
    }
}
