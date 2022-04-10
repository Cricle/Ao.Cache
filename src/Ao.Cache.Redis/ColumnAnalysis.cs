using Ao.ObjectDesign;
using Ao.Cache.Redis.Annotations;
using Ao.Cache.Redis.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Ao.Cache.Redis
{
    public class CacheColumnAnalysis : ICacheColumnAnalysis
    {
        public static readonly Type StringType = typeof(string);
        public static readonly string IListName = typeof(IList).FullName;
        public static readonly string IDictionaryName = typeof(IDictionary).FullName;

        public CacheColumnAnalysis()
        {
            IgnoreTypes = new HashSet<Type>();
            IgnoreProperties = new HashSet<PropertyInfo>();
            IgnoreNames = new HashSet<string>();
            ConverterProviders = new List<IConverterProvider>();
        }

        public List<IConverterProvider> ConverterProviders { get; }

        public HashSet<Type> IgnoreTypes { get; }

        public HashSet<PropertyInfo> IgnoreProperties { get; }

        public HashSet<string> IgnoreNames { get; }

        public bool IgnoreNoSetter { get; set; }

        public IReadOnlyDictionary<string, ICacheColumn> GetRedisColumnMap(Type type, string prefx)
        {
            var columns = GetRedisColumns(type, prefx);
            var map = columns.ToDictionary(x => x.Name);
            return map;
        }

        public IReadOnlyList<ICacheColumn> GetRedisColumns(Type type, string prefx)
        {
            if (type.IsPrimitive || type == typeof(string))
            {
                throw new ArgumentException($"Type {type} can't parse!");
            }
            return Analysis(type, prefx);
        }
        protected virtual bool CanLookup(PropertyInfo info)
        {
            return info.GetCustomAttribute<IgnoreColumnAttribute>() == null &&
                info.GetIndexParameters().Length == 0 &&
                !IgnoreTypes.Contains(info.PropertyType) &&
                !IgnoreProperties.Contains(info) &&
                !IgnoreNames.Contains(info.Name);
        }
        protected virtual bool CanDeep(PropertyInfo info, ICacheColumn column)
        {
            return !info.PropertyType.IsValueType && info.PropertyType != StringType &&
                info.GetCustomAttribute<NotDeepAttribute>() == null &&
                info.PropertyType.GetInterface(IListName) == null &&
                info.PropertyType.GetInterface(IDictionaryName) == null;
        }
        protected virtual ICacheValueConverter CreateConverter(PropertyInfo info)
        {
            return KnowsCacheValueConverter.GetConverter(info.PropertyType);
        }
        protected readonly Dictionary<Type, ICacheValueConverter> convertTypeCache = new Dictionary<Type, ICacheValueConverter>();
        protected virtual bool TryGetConverter(Type instanceType,PropertyInfo info,out ICacheValueConverter converter)
        {
            foreach (var item in ConverterProviders)
            {
                converter = item.GetConverter(instanceType, info);
                if (converter != null) 
                {
                    return true;
                }
            }
            var converterAttr = info.GetCustomAttribute<CacheValueConverterAttribute>();
            if (converterAttr == null)
            {
                converter = null;
                return false;
            }
            if (!convertTypeCache.TryGetValue(converterAttr.ConvertType, out converter))
            {
                var members = converterAttr.ConvertType.GetMember("Instance", BindingFlags.Static | BindingFlags.Public);
                if (members.Length == 0)
                {
                    converter = (ICacheValueConverter)ReflectionHelper.Create(converterAttr.ConvertType);
                }
                else
                {
                    var member = members[0];
                    if (member.MemberType == MemberTypes.Field)
                    {
                        converter = (ICacheValueConverter)((FieldInfo)member).GetValue(null);
                    }
                    else if (member.MemberType == MemberTypes.Property)
                    {
                        converter = (ICacheValueConverter)((PropertyInfo)member).GetValue(null);
                    }
                    else
                    {
                        throw new InvalidOperationException("Error to get static member value");
                    }
                }
                convertTypeCache[converterAttr.ConvertType] = converter;
                return true;
            }
            return false;
        }
        protected virtual ICacheValueConverter ConverterNotFound(Type type,PropertyInfo property)
        {
            ThrowConverterNotFound(type, property);
            return null;
        }
        private void ThrowConverterNotFound(Type type,PropertyInfo property)
        {
            throw new InvalidOperationException($"Type {type.FullName} property {property.Name} can't get a converter");
        }
        private ICacheColumn[] Analysis(Type type, string prefx)
        {            
            var columns = new List<ICacheColumn>();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(CanLookup);
            var convertTypeCache = new Dictionary<Type, ICacheValueConverter>();
            var nameSet = new HashSet<string>();

            foreach (var item in props)
            {
                if(!TryGetConverter(type,item,out var converter))
                {
                    converter = CreateConverter(item);
                }
                if (converter==null)
                {
                    ConverterNotFound(type, item);
                }
                if (converter==null)
                {
                    ThrowConverterNotFound(type, item);
                }

                PropertyGetter getter = null;
                PropertySetter setter = null;

                var identity = new PropertyIdentity(item);

                if (item.CanRead)
                {
                    getter = CompiledPropertyInfo.GetGetter(identity);
                }
                if (item.CanWrite)
                {
                    setter = CompiledPropertyInfo.GetSetter(identity);
                }
                else
                {
                    if (IgnoreNoSetter)
                    {
                        continue;
                    }
                }
                var name = item.Name;
                var nameAttr = item.GetCustomAttribute<ColumnAttribute>();
                if (nameAttr != null)
                {
                    name = nameAttr.Name;
                }
                if (!nameSet.Add(name))
                {
                    throw new ArgumentException($"Name {name} in type {type} is not only");
                }
                var column = new CacheColumn
                {
                    Converter = converter,
                    Getter = getter,
                    Setter = setter,
                    Name = name,
                    Path = string.IsNullOrEmpty(prefx) ? name : string.Concat(prefx, ".", name),
                    Property = item,
                    NameRedis = name,
                };
                columns.Add(column);
                if (CanDeep(item, column))
                {
                    var nexts = Analysis(item.PropertyType, name);
                    column.Nexts = nexts;
                }
            }
            return columns.ToArray();
        }

    }
}
