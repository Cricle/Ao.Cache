using Ao.ObjectDesign;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Ao.Cache.InRedis.HashList
{
    public abstract class EntryCacheOperator : IHashCacheOperator, IAutoWriteCache, IEntryCacheOperator, IListCacheOperator
    {
        public static readonly RedisValue defaultName = new RedisValue("Default");

        private bool isValueType;
        private bool isString;
        private bool isObject;
        private TypeCreator typeCreator;

        public Type Target { get; }

        public EntryCacheOperator(Type target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public virtual void Build()
        {
            isValueType = Target.IsValueType;
            isString = Target == typeof(string);
            isObject = !isString && Target.IsClass;
            if (isObject)
            {
                typeCreator = CompiledPropertyInfo.GetCreator(Target);
            }
        }


        HashEntry[] ICacheOperator<HashEntry[]>.As(object value)
        {
            return new HashEntry[]
            {
                new HashEntry(defaultName, AsCore(value))
            };
        }

        public void Write(ref object instance, HashEntry[] entries)
        {
            if (entries.Length != 0)
            {
                WriteCore(ref instance, entries[0].Value);
            }
        }
        public object Write(HashEntry[] entries)
        {
            var instance = CreateInstance();
            Write(ref instance, entries);
            return instance;
        }
        public object Write(in RedisValue value)
        {
            var instance = CreateInstance();
            Write(ref instance, value);
            return instance;
        }
        protected abstract void WriteCore(ref object instance, in RedisValue entry);
        protected abstract RedisValue AsCore(object value);

        protected virtual object CreateInstance()
        {
            if (isValueType)
            {
                if (!structCache.TryGetValue(Target, out var val))
                {
                    val = Activator.CreateInstance(Target);
                    structCache[Target] = val;
                }
                return val;
            }
            if (isString)
            {
                return string.Empty;
            }
            return typeCreator?.Invoke();
        }

        public void Write(ref object instance, RedisValue entry)
        {
            WriteCore(ref instance, entry);
        }

        public RedisValue As(object value)
        {
            return AsCore(value);
        }

        public void Write(ref object instance, RedisValue[] entries)
        {
            WriteCore(ref instance, entries[0]);
        }

        RedisValue[] ICacheOperator<RedisValue[]>.As(object value)
        {
            return new RedisValue[]
            {
                AsCore(value),
            };
        }

        private static readonly Dictionary<Type, object> structCache = new Dictionary<Type, object>();
    }
}
