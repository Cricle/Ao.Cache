using FastExpressionCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ao.Cache.InRedis.HashList.Converters;
using StackExchange.Redis;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Ao.Cache.InRedis.HashList
{
    public class ExpressionHashCacheOperator: ComplexCacheOperator,IHashCacheOperator
    {
        private static readonly Dictionary<Type, ExpressionHashCacheOperator> defaultRedisOpCache = new Dictionary<Type, ExpressionHashCacheOperator>();

        public static ExpressionHashCacheOperator GetRedisOperator(Type type)
        {
            if (!defaultRedisOpCache.TryGetValue(type, out var @operator))
            {
                @operator = new ExpressionHashCacheOperator(type, SharedAnalysis);
                defaultRedisOpCache[type] = @operator;
                @operator.Build();
            }
            return @operator;
        }
        private static readonly MethodInfo TryGetValueMethod = typeof(IDictionary<string, RedisValue>).GetMethod("TryGetValue");
        private static readonly ConstructorInfo HashEntryConstructorInfo = typeof(HashEntry).GetConstructor(new Type[] { typeof(RedisValue), typeof(RedisValue) });
        private static readonly MethodInfo ConvertMethod = typeof(ICacheValueConverter).GetMethod("Convert");
        private static readonly MethodInfo ConvertBackMethod = typeof(ICacheValueConverter).GetMethod("ConvertBack");

        public ExpressionHashCacheOperator(Type target, ICacheColumnAnalysis columnAnalysis) 
            : base(target, columnAnalysis)
        {
        }
        
        private Action<object, IDictionary<string, RedisValue>> writeMethod;
        private Func<object, HashEntry[]> asMethod;
        private Func<IDictionary<string, RedisValue>,object> writeWithObjectMethod;

        protected override void OnBuild()
        {
            writeMethod = AotCompileWrite();
            asMethod = AotCompileAs();
            writeWithObjectMethod = AotCompileWithInstanceWrite();
        }
        private IEnumerable<Expression> AotWriteAll(Expression instance, IEnumerable<ICacheColumn> columns, Expression map)
        {
            foreach (var column in columns)
            {
                var val = Expression.Variable(typeof(RedisValue));
                var value = Expression.Variable(typeof(object));
                var tryGet = Expression.Call(map, TryGetValueMethod, Expression.Constant(column.Path), val);
                Expression assignValue = Expression.Assign(value, Expression.Convert(val, typeof(object)));
                if (column.Converter != null)
                {
                    assignValue = Expression.Assign(value, Expression.Call(
                        Expression.Constant(column.Converter),
                        ConvertBackMethod,
                        val,
                        Expression.Constant(column)));
                }
                var doNothingCheck = Expression.IfThen(
                    Expression.NotEqual(value, Expression.Constant(CacheValueConverterConst.DoNothing)),
                    Expression.Call(instance, column.Property.SetMethod, Expression.Convert(value, column.Property.PropertyType)));
                var ifThen = Expression.IfThen(
                    Expression.Equal(tryGet, Expression.Constant(true)),
                    Expression.Block(assignValue, doNothingCheck));
                yield return Expression.Block(new ParameterExpression[] { val, value }, new Expression[] { ifThen });
                if (column.Converter == null && column.Nexts != null && column.Nexts.Count != 0)
                {
                    yield return Expression.Call(instance, column.Property.SetMethod, Expression.New(column.Property.PropertyType));
                    var nextInst = Expression.Call(instance, column.Property.GetMethod);
                    foreach (var item in AotWriteAll(nextInst, column.Nexts, map))
                    {
                        yield return item;
                    }
                }
            }
        }
        private Func<IDictionary<string, RedisValue>,object> AotCompileWithInstanceWrite()
        {
            var map = Expression.Parameter(typeof(IDictionary<string, RedisValue>));
            var inst = Expression.Variable(Target);
            var assign = Expression.Assign(inst, Expression.New(Target));
            var exps = AotWriteAll(inst, Columns, map);
            var allExps = new List<Expression> { assign };
            allExps.AddRange(exps);
            allExps.Add(Expression.Convert(inst, Target));
            var body = Expression.Block(new ParameterExpression[] {inst},allExps);
            return Expression.Lambda<Func<IDictionary<string, RedisValue>, object>>(body, map)
                .CompileSys();
        }
        private Action<object,IDictionary<string, RedisValue>> AotCompileWrite()
        {
            var map = Expression.Parameter(typeof(IDictionary<string, RedisValue>));
            var inst = Expression.Parameter(typeof(object));
            var exps = AotWriteAll(Expression.Convert(inst,Target), Columns, map);
            var allExps = new List<Expression>();
            allExps.AddRange(exps);
            allExps.Add(inst);
            var body = Expression.Block(allExps);
            return Expression.Lambda<Action<object, IDictionary<string, RedisValue>>>(body, inst,map)
                .CompileSys();
        }
        private Func<object, HashEntry[]> AotCompileAs()
        {
            var inst = Expression.Parameter(typeof(object));
            var exps = CompileGetEntities(Expression.Convert(inst, Target), Columns);
            var listExp = Expression.NewArrayInit(typeof(HashEntry), exps);
            return Expression.Lambda<Func<object, HashEntry[]>>(listExp, inst)
                .CompileFast();
        }
        private IEnumerable<Expression> CompileGetEntities(Expression instance, IEnumerable<ICacheColumn> columns)
        {
            foreach (var column in columns)
            {
                var val = Expression.Call(instance, column.Property.GetMethod);
                if (column.Nexts != null && column.Nexts.Count != 0)
                {
                    foreach (var item in CompileGetEntities(val, column.Nexts))
                    {
                        yield return item;
                    }
                    continue;
                }
                if (column.Converter == null)
                {
                    yield return Expression.Constant(new HashEntry(column.Path, RedisValue.EmptyString));
                }
                else
                {
                    var call = Expression.Call(Expression.Constant(column.Converter), ConvertMethod,
                        instance,
                        Expression.Convert(Expression.Call(instance, column.Property.GetMethod), typeof(object)),
                        Expression.Constant(column));
                    yield return Expression.New(HashEntryConstructorInfo,
                        Expression.Constant(new RedisValue(column.Path)),
                        call);
                }
            }
        }
        public object Write(HashEntry[] entries)
        {
            return writeWithObjectMethod(ToMap(entries));
        }
        private static Dictionary<string, RedisValue> ToMap(HashEntry[] entries)
        {
            var len = entries.Length;
            var d = new Dictionary<string, RedisValue>(len);
            for (int i = 0; i < len; i++)
            {
                var item = entries[i];
                d.Add(item.Name.ToString(), item.Value);
            }
            return d;
        }
        public void Write(ref object instance, HashEntry[] entries)
        {
            var map = ToMap(entries);
            writeMethod(instance, map);
        }

        public HashEntry[] As(object value)
        {
            return asMethod(value);
        }
    }
}
