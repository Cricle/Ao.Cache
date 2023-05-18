using System.Linq.Expressions;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Ao.Cache
{
    public static class CacheHelperCreatorFetchHelper
    {
        public static bool SetInCache<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp, T value)
        {
            var finder = GetFinderSync(creator, exp);
            var identity = GetIdentity(exp);
            return finder.SetInCache(identity, value);
        }
        public static T FindInCache<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp)
        {
            var finder = GetFinderSync(creator,exp);
            var identity = GetIdentity(exp);
            return finder.FindInCache(identity);
        }
        public static bool Exists<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp)
        {
            var finder = GetFinderSync(creator, exp);
            var identity = GetIdentity(exp);
            return finder.Exists(identity);
        }
        public static bool Renewal<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp, TimeSpan? time)
        {
            var finder = GetFinderSync(creator, exp);
            var identity = GetIdentity(exp);
            return finder.Renewal(identity, time);
        }
        public static bool Renewal<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp)
        {
            var finder = GetFinderSync(creator, exp);
            var identity = GetIdentity(exp);
            return finder.Renewal(identity);
        }
        public static bool Delete<
#if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods| DynamicallyAccessedMemberTypes.PublicConstructors| DynamicallyAccessedMemberTypes.NonPublicMethods)]
#endif
            T>(this ICacheHelperCreator creator,Expression<Func<T>> exp)
        {
            var finder = GetFinderSync(creator, exp);
            var identity = GetIdentity(exp);
            return finder.Delete(identity);
        }

        public static Task<bool> SetInCacheAsync<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp, T value)
        {
            var finder = GetFinder(creator, exp);
            var identity = GetIdentity(exp);
            return finder.SetInCacheAsync(identity, value);
        }
        public static Task<T> FindInCacheAsync<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp)
        {
            var finder = GetFinder(creator, exp);
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
        public static ISyncDataFinder<string, T> GetFinderSync<T>(this ICacheHelperCreator creator, Expression<Func<T>> exp)
        {
            return creator.GetHelper<T>().GetFinderSync(exp);
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
                        continue;
                    }
                    else if (
#if NET6_0_OR_GREATER
                        RuntimeFeature.IsDynamicCodeSupported&&
#endif
                        arg is UnaryExpression unaryExp&& unaryExp.Operand is ConstantExpression unaryConstExp)
                    {
                        if (unaryExp.Type.IsGenericType && unaryExp.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            if (unaryConstExp.Value != null)
                            {
                                args[i] = Convert.ChangeType(unaryConstExp.Value, unaryExp.Type.GenericTypeArguments[0]);
                            }
                        }
                        else
                        {
                            args[i] = Convert.ChangeType(unaryConstExp.Value, unaryExp.Type);
                        }
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
}
