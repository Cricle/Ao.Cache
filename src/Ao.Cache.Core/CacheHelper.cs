#if !NETSTANDARD1_0
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System;
using Ao.Cache.Core.Annotations;

namespace Ao.Cache
{
    internal readonly struct DeclareInfo : IEquatable<DeclareInfo>
    {
        public readonly Type Type;

        public readonly MethodInfo Method;

        public DeclareInfo(Type type, MethodInfo method)
        {
            Type = type;
            Method = method;
        }

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            return Type?.GetHashCode() ?? 0 ^ Method?.GetHashCode() ?? 0;
#else
                return HashCode.Combine(Type, Method);
#endif
        }
        public override bool Equals(object obj)
        {
            if (obj is DeclareInfo info)
            {
                return Equals(info);
            }
            return false;
        }

        public bool Equals(DeclareInfo other)
        {
            return other.Type == Type && other.Method == Method;
        }
    }

    public class CacheHelper<TReturn>
    {
        private readonly Dictionary<DeclareInfo, IDataFinder<string, TReturn>> finders = new Dictionary<DeclareInfo, IDataFinder<string, TReturn>>();

        private readonly object locker = new object();

        public CacheHelper(IDataFinderFactory factory)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IDataFinderFactory Factory { get; }

        private Type GetDeclareType(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }
            if (expression is NewExpression newExp)
            {
                return newExp.Constructor.DeclaringType;
            }
            if (expression is MemberExpression memberExp)
            {
                if (memberExp.Member is FieldInfo field)
                {
                    return field.FieldType;
                }
                else if (memberExp.Member is PropertyInfo property)
                {
                    return property.PropertyType;
                }
            }
            if (expression is ConstantExpression constExp && constExp.Value != null)
            {
                return constExp.Value.GetType();
            }
            return null;
        }
        public IDataFinder<string, TReturn> GetFinder(Type instanceType, MethodInfo method)
        {
            var key = new DeclareInfo(instanceType, method);
            if (!finders.TryGetValue(key, out var finder))
            {
                lock (locker)
                {
                    if (!finders.TryGetValue(key, out finder))
                    {
                        CacheProxyAttribute declareAttr = null;
                        if (declareAttr != null)
                        {
                            declareAttr = instanceType.GetCustomAttribute<CacheProxyAttribute>();
                        }
                        var proxyAttr = method.GetCustomAttribute<CacheProxyMethodAttribute>();
                        finder = Factory.CreateEmpty<string, TReturn>();
                        if (proxyAttr != null)
                        {
                            var head = proxyAttr.Head;
                            if (!proxyAttr.HeadAbsolute)
                            {
                                head = (string.IsNullOrEmpty(declareAttr?.Head) ? string.Empty : ".") + head;
                            }
                            if (!string.IsNullOrEmpty(head))
                            {
                                finder.Options.WithHead(head);
                            }
                            if (TimeSpan.TryParse(proxyAttr.CacheTime, out var tp))
                            {
                                finder.Options.WithCacheTime(tp);
                            }
                            if (proxyAttr.Renewal)
                            {
                                finder.Options.WithRenew(true);
                            }
                        }
                        finders.Add(key, finder);
                    }
                }
            }
            return finder;
        }
        public IDataFinder<string, TReturn> GetFinder(Expression<Func<TReturn>> exp)
        {
            if (exp.Body is MethodCallExpression methodCall)
            {
                var declareType = GetDeclareType(methodCall.Object);
                return GetFinder(declareType, methodCall.Method);
            }
            throw new NotSupportedException(exp.ToString());
        }

    }

}
#endif