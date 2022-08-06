using Apex.Serialization;
using Microsoft.IO;
using System;
using System.Collections.Generic;

namespace Ao.Cache.Serizlier.Apex
{
    public class ApexEntityConvertor<TEntity> : IEntityConvertor<TEntity>
    {
        private static readonly IBinary DefaultBinary = ApexEntityConvertor.GetBinary(typeof(TEntity));

        public static readonly ApexEntityConvertor<TEntity> Default = new ApexEntityConvertor<TEntity>();
        
        public ApexEntityConvertor()
            :this(DefaultBinary)
        {
        }

        public ApexEntityConvertor(IBinary binary)
        {
            Binary = binary ?? throw new ArgumentNullException(nameof(binary));
        }

        public IBinary Binary { get; }

        public byte[] ToBytes(TEntity entry)
        {
            using (var mem = ApexEntityConvertor.streamManager.GetStream())
            {
                Binary.Write(entry, mem);
                return mem.ToArray();
            }
        }

        public TEntity ToEntry(byte[] bytes)
        {
            using (var mem = ApexEntityConvertor.streamManager.GetStream(bytes))
            {
                return Binary.Read<TEntity>(mem);
            }
        }
    }
    public class ApexEntityConvertor : IEntityConvertor
    {
        internal static readonly RecyclableMemoryStreamManager streamManager = new RecyclableMemoryStreamManager();
        private static readonly Dictionary<Type, IBinary> binaryCache = new Dictionary<Type, IBinary>();
        private static readonly object locker = new object();
        public static readonly ApexEntityConvertor Default = new ApexEntityConvertor();

        public ApexEntityConvertor()
        {
        }

        public static IBinary GetBinary(Type type)
        {
            if (!binaryCache.TryGetValue(type,out var binary))
            {
                lock (locker)
                {
                    if (!binaryCache.TryGetValue(type, out binary))
                    {
                        var settings = new Settings { AllowFunctionSerialization = false, SupportSerializationHooks = false, UseSerializedVersionId = false };
                        settings = settings.MarkSerializable(type);
                        binary = Binary.Create(settings);
                        binaryCache[type] = binary;
                    }
                }
            }
            return binary;
        }
        protected virtual IBinary SelectBinary(Type type)
        {
            return GetBinary(type);
        }
        public byte[] ToBytes(object entry, Type type)
        {
            using (var mem = streamManager.GetStream())
            {
                SelectBinary(type).Write(entry, mem);
                return mem.ToArray();
            }
        }

        public object ToEntry(byte[] bytes, Type type)
        {
            using (var mem = streamManager.GetStream(bytes))
            {
                return SelectBinary(type).Read<object>(mem);
            }
        }
    }
}
