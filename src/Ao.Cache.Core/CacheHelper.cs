using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System;
using Ao.Cache.Core.Annotations;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

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
    public interface ICacheHelperCreator
    {
        ICacheHelper<TReturn> GetHelper<TReturn>();
    }
    public static class CacheHelperCreatorFetchHelper
    {
        public static Task<bool> SetInCacheAsync<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp, T value)
        {
            var finder = creator.GetHelper<T>().GetFinder(exp);
            var identity = GetIdentity(exp);
            return finder.SetInCacheAsync(identity, value);
        }
        public static Task<T> FindInCacheAsync<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp)
        {
            var finder = creator.GetHelper<T>().GetFinder(exp);
            var identity = GetIdentity(exp);
            return finder.FindInCacheAsync(identity);
        }
        public static Task<bool> ExistsAsync<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp)
        {
            var finder = GetFinder(creator, exp);
            var identity = GetIdentity(exp);
            return finder.ExistsAsync(identity);
        }
        public static Task<bool> RenewalAsync<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp, TimeSpan? time)
        {
            var finder = GetFinder(creator, exp);
            var identity = GetIdentity(exp);
            return finder.RenewalAsync(identity, time);
        }
        public static Task<bool> RenewalAsync<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp)
        {
            var finder = GetFinder(creator, exp);
            var identity = GetIdentity(exp);
            return finder.RenewalAsync(identity);
        }
        public static Task<bool> DeleteAsync<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp)
        {
            var finder = GetFinder(creator, exp);
            var identity = GetIdentity(exp);
            return finder.DeleteAsync(identity);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDataFinder<string, T> GetFinder<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp)
        {
            return creator.GetHelper<T>().GetFinder(exp);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetIdentity(object[] args)
        {
            return string.Join(",", args);
        }
        public static string GetIdentity<T>(Expression<Func<T>> exp)
        {
            if (exp.Body is MethodCallExpression methodCall)
            {
                var args = new object[methodCall.Arguments.Count];
                for (int i = 0; i < methodCall.Arguments.Count; i++)
                {
                    var arg = methodCall.Arguments[i];
                    if (arg is ConstantExpression constExp)
                    {
                        args[i] = constExp.Value;
                    }
                    else
                    {
                        args[i] = Expression.Lambda(arg).Compile().DynamicInvoke();
                    }
                }
                return string.Join(",", args);
            }
            throw new NotSupportedException(exp.ToString());
        }
    }
    public class CacheHelperCreator : ICacheHelperCreator
    {
        public CacheHelperCreator(IDataFinderFactory factory)
        {
            Factory = factory;
        }

        public IDataFinderFactory Factory { get; }

        public ICacheHelper<TReturn> GetHelper<TReturn>()
        {
            return CacheHelperStore<TReturn>.Get(Factory);
        }
        static class CacheHelperStore<TReturn>
        {
            private static ICacheHelper<TReturn> instance;
            private static readonly object locker = new object();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ICacheHelper<TReturn> Get(IDataFinderFactory factory)
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new CacheHelper<TReturn>(factory);
                        }
                    }
                }
                return instance;
            }
        }
    }
    public interface ICacheHelper<TReturn>
    {
        IDataFinder<string, TReturn> GetFinder(Type instanceType, MethodInfo method);

        IDataFinder<string, TReturn> GetFinder(Expression<Func<TReturn>> exp);
    }
    public class CacheHelper<
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    TReturn> : ICacheHelper<TReturn>
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
                        finder = Factory.Create<string, TReturn>();
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
                            }
                        }
                        if (string.IsNullOrEmpty(head))
                        {
                            head = $"{instanceType.FullName}.{method.Name}[{method.GetGenericArguments().Length}]({method.GetParameters().Length})";
                        }
                        finder.Options.WithHead(head);
                        if (proxyAttr != null && proxyAttr.Renewal)
                        {
                            finder.Options.WithRenew(true);
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
