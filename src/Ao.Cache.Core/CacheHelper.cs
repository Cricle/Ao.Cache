using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using Ao.Cache.Annotations;

namespace Ao.Cache
{
    public class CacheHelper<
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
        TReturn> : ICacheHelper<TReturn>
    {
        private readonly Dictionary<DeclareInfo, Finders<TReturn>> finders = new Dictionary<DeclareInfo, Finders<TReturn>>();

        private readonly object locker = new object();

        public CacheHelper(IDataFinderFactory factory, ISyncDataFinderFactory syncFactory)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            SyncFactory=syncFactory ?? throw new ArgumentNullException(nameof(syncFactory));
        }

        public IDataFinderFactory Factory { get; }

        public ISyncDataFinderFactory SyncFactory { get; }

        private static Type GetDeclareType(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }
            Type ret = null;
            if (expression is NewExpression newExp)
            {
                ret = newExp.Constructor.DeclaringType;
            }
            else if (expression is MemberExpression memberExp)
            {
                if (memberExp.Member is FieldInfo field)
                {
                    ret = field.FieldType;
                }
                else if (memberExp.Member is PropertyInfo property)
                {
                    ret = property.PropertyType;
                }
            }
            else if (expression is ConstantExpression constExp && constExp.Value != null)
            {
                ret = constExp.Value.GetType();
            }
            if (ret != null)
            {
                var proxy = ret.GetCustomAttribute<CacheProxyByAttribute>();
                if (proxy != null)
                {
                    ret = proxy.ProxyType;
                }
            }
            return ret;
        }
        private Finders<TReturn> GetFinders(Type instanceType, MethodInfo method)
        {
            var key = new DeclareInfo(instanceType, method);
            if (!this.finders.TryGetValue(key, out var finders))
            {
                lock (locker)
                {
                    if (!this.finders.TryGetValue(key, out finders))
                    {
                        CacheProxyAttribute declareAttr = null;
                        if (declareAttr != null)
                        {
                            declareAttr = instanceType.GetCustomAttribute<CacheProxyAttribute>();
                        }
                        var proxyAttr = method.GetCustomAttribute<CacheProxyMethodAttribute>();
                        var finder = Factory.Create<string, TReturn>();
                        var syncFinder = SyncFactory.CreateSync<string, TReturn>();
                        string head = null;
                        if (proxyAttr != null)
                        {
                            head = proxyAttr.Head;
                            if (!proxyAttr.HeadAbsolute)
                            {
                                head = (string.IsNullOrEmpty(declareAttr?.Head) ? string.Empty : ".") + head;
                            }
                            if (TimeSpan.TryParse(proxyAttr.CacheTime, out var tp))
                            {
                                finder.Options.WithCacheTime(tp);
                                syncFinder.Options.WithCacheTime(tp);
                            }
                        }
                        if (string.IsNullOrEmpty(head))
                        {
                            head = $"{instanceType.FullName}.{method.Name}[{method.GetGenericArguments().Length}]({method.GetParameters().Length})";
                        }
                        finder.Options.WithHead(head);
                        syncFinder.Options.WithHead(head);
                        if (proxyAttr != null && proxyAttr.Renewal)
                        {
                            finder.Options.WithRenew(true);
                            syncFinder.Options.WithRenew(true);
                        }
                        finders = new Finders<TReturn>(finder, syncFinder);
                        this.finders.Add(key, finders);
                    }
                }
            }
            return finders;
        }
        public IDataFinder<string, TReturn> GetFinder(Type instanceType, MethodInfo method)
        {
            return GetFinders(instanceType, method).Finder;
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

        public ISyncDataFinder<string, TReturn> GetFinderSync(Type instanceType, MethodInfo method)
        {
            return GetFinders(instanceType, method).SyncFinder;
        }

        public ISyncDataFinder<string, TReturn> GetFinderSync(Expression<Func<TReturn>> exp)
        {
            if (exp.Body is MethodCallExpression methodCall)
            {
                var declareType = GetDeclareType(methodCall.Object);
                return GetFinderSync(declareType, methodCall.Method);
            }
            throw new NotSupportedException(exp.ToString());
        }
    }

}
