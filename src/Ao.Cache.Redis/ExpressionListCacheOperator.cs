using FastExpressionCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastExpressionCompiler.LightExpression;
using Ao.Cache.Redis.Converters;
using StackExchange.Redis;

namespace Ao.Cache.Redis
{
    public class ExpressionListCacheOperator : ComplexCacheOperator,IListCacheOperator
    {
        private static readonly Dictionary<Type, ExpressionListCacheOperator> defaultRedisOpCache = new Dictionary<Type, ExpressionListCacheOperator>();

        public static ExpressionListCacheOperator GetRedisOperator(Type type)
        {
            if (!defaultRedisOpCache.TryGetValue(type, out var @operator))
            {
                @operator = new ExpressionListCacheOperator(type, SharedAnalysis);
                defaultRedisOpCache[type] = @operator;
                @operator.Build();
            }
            return @operator;
        }

        private static readonly MethodInfo ConvertMethod = typeof(ICacheValueConverter).GetMethod("Convert");
        private static readonly MethodInfo ConvertBackMethod = typeof(ICacheValueConverter).GetMethod("ConvertBack");

        public ExpressionListCacheOperator(Type target, ICacheColumnAnalysis columnAnalysis) : base(target, columnAnalysis)
        {
        }

        private Action<object, RedisValue[]> writeMethod;
        private Func<object, RedisValue[]> asMethod;
        private Func<RedisValue[], object> writeWithObjectMethod;

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
                var index = Array.IndexOf((ICacheColumn[])NoNextAllColumns, column);
                if (index != -1)
                {
                    var val = Expression.Variable(typeof(RedisValue));
                    var value = Expression.Variable(typeof(object));
                    var valAssign = Expression.Assign(val, Expression.ArrayIndex(map, Expression.Constant(index)));
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
                    yield return Expression.Block(new ParameterExpression[] { val, value },
                        new Expression[] { valAssign, assignValue, doNothingCheck });
                }
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
        private Func<RedisValue[], object> AotCompileWithInstanceWrite()
        {
            var map = Expression.Parameter(typeof(RedisValue[]));
            var inst = Expression.Variable(Target);
            var assign = Expression.Assign(inst, Expression.New(Target));
            var exps = AotWriteAll(inst, Columns, map);
            var allExps = new List<Expression> { assign };
            allExps.AddRange(exps);
            allExps.Add(Expression.Convert(inst, Target));
            var body = Expression.Block(new ParameterExpression[] { inst }, allExps);
            return Expression.Lambda<Func<RedisValue[], object>>(body, map)
                .CompileSys();
        }
        private Action<object, RedisValue[]> AotCompileWrite()
        {
            var map = Expression.Parameter(typeof(RedisValue[]));
            var inst = Expression.Parameter(typeof(object));
            var exps = AotWriteAll(Expression.Convert(inst, Target), Columns, map);
            var allExps = new List<Expression>();
            allExps.AddRange(exps);
            allExps.Add(inst);
            var body = Expression.Block(allExps);
            return Expression.Lambda<Action<object, RedisValue[]>>(body, inst, map)
                .CompileSys();
        }
        private Func<object, RedisValue[]> AotCompileAs()
        {
            var inst = Expression.Parameter(typeof(object));
            var exps = CompileGetEntities(Expression.Convert(inst, Target), Columns);
            var listExp = Expression.NewArrayInit(typeof(RedisValue), exps);
            return Expression.Lambda<Func<object, RedisValue[]>>(listExp, inst)
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
                    yield return Expression.Constant(RedisValue.EmptyString);
                }
                else
                {
                    var call = Expression.Call(Expression.Constant(column.Converter), ConvertMethod,
                        instance,
                        Expression.Convert(Expression.Call(instance, column.Property.GetMethod), typeof(object)),
                        Expression.Constant(column));
                    yield return call;
                }
            }
        }
        public object Write(RedisValue[] entries)
        {
            return writeWithObjectMethod(entries);
        }
        public void Write(ref object instance, RedisValue[] entries)
        {
            writeMethod(instance, entries);
        }

        public RedisValue[] As(object value)
        {
            return asMethod(value);
        }
    }
}
