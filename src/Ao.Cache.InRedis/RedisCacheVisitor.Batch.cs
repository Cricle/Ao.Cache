using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ao.Cache.InRedis
{
    public partial class RedisCacheVisitor
    {
        interface IInputCase<TInput, TOutput>
        {
            TOutput Case(in TInput input);
        }
        class RedisStringInputCase : IInputCase<RedisValue, string>
        {
            public static readonly RedisStringInputCase Default = new RedisStringInputCase();

            public string Case(in RedisValue input)
            {
                return input;
            }
        }
        class TransferInputCase<T> : IInputCase<RedisValue, T>
        {
            public TransferInputCase(IObjectTransfer objectTransfer)
            {
                ObjectTransfer = objectTransfer;
            }

            public IObjectTransfer ObjectTransfer { get; }

            public T Case(in RedisValue input)
            {
                return ObjectTransfer.Transfer<T>(input);
            }
        }
        class NullInputCase<T> : IInputCase<T, T>
        {
            public static readonly NullInputCase<T> Default = new NullInputCase<T>();

            public T Case(in T input)
            {
                return input;
            }
        }
        readonly struct BatchResult<T>
        {
            public readonly Task Task;

            public readonly Task<T>[] Results;

            public BatchResult(Task task, Task<T>[] results)
            {
                Task = task;
                Results = results;
            }
        }
        private BatchResult<T> CreateBatch<T>(Func<IBatch, Task<T>[]> creator)
        {
            var batch = Database.CreateBatch();
            var tasks = creator(batch);
            batch.Execute();
            var newTask = Task.WhenAll(tasks);
            return new BatchResult<T>(newTask, tasks);
        }
        private RedisKey[] AsKeys(IReadOnlyList<string> keys)
        {
            var map = new RedisKey[keys.Count];
            for (int i = 0; i < keys.Count; i++)
            {
                map[i] = keys[i];
            }
            return map;
        }
        private IDictionary<string, T> CreateMap<T>(IReadOnlyList<string> keys, Task<T>[] inputs)
        {
            return CreateMap(keys, inputs, NullInputCase<T>.Default);
        }
        private IDictionary<string, T> CreateMap<T>(IReadOnlyList<string> keys, RedisValue[] values)
        {
            var map = new Dictionary<string, T>(keys.Count);
            for (int i = 0; i < values.Length; i++)
            {
                map[keys[i]] = ObjectTransfer.Transfer<T>(values[i]);
            }
            return map;
        }
        private IDictionary<string, TOutput> CreateMap<TInput, TOutput>(IReadOnlyList<string> keys,
            Task<TInput>[] inputs, IInputCase<TInput, TOutput> inputCase)
        {
            var map = new Dictionary<string, TOutput>(keys.Count);
            for (int i = 0; i < keys.Count; i++)
            {
                map[keys[i]] = inputCase.Case(inputs[i].Result);
            }
            return map;
        }
        public IDictionary<string, bool> Exists(IReadOnlyList<string> keys)
        {
            return ExistsAsync(keys).GetAwaiter().GetResult();
        }

        public async Task<IDictionary<string, bool>> ExistsAsync(IReadOnlyList<string> keys)
        {
            var task = CreateBatch(x => keys.Select(y => x.KeyExistsAsync(y)).ToArray());
            await task.Task;
            return CreateMap(keys, task.Results);
        }

        public IDictionary<string, T> Get<T>(IReadOnlyList<string> keys)
        {
            return GetAsync<T>(keys).GetAwaiter().GetResult();
        }
        public async Task<IDictionary<string, T>> GetAsync<T>(IReadOnlyList<string> keys)
        {
            var redisKeys = AsKeys(keys);
            var res = await Database.StringGetAsync(redisKeys);
            return CreateMap<T>(keys, res);
        }

        public IDictionary<string, bool> Set<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            return SetAsync(datas, cacheTime, cacheSetIf).GetAwaiter().GetResult();
        }
        public async Task<IDictionary<string, bool>> SetAsync<T>(KeyValuePair<string, T>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var task = CreateBatch(x =>
                datas.Select(y =>
                    x.StringSetAsync(y.Key, ObjectTransfer.Transfer(y.Value), cacheTime, (When)cacheSetIf)).ToArray());
            await task.Task;
            var map = new Dictionary<string, bool>(datas.Length);
            for (int i = 0; i < task.Results.Length; i++)
            {
                map[datas[i].Key] = task.Results[i].Result;
            }
            return map;
        }

        public IDictionary<string, string> GetString(IReadOnlyList<string> keys)
        {
            return GetStringAsync(keys).GetAwaiter().GetResult();
        }

        public async Task<IDictionary<string, string>> GetStringAsync(IReadOnlyList<string> keys)
        {
            var task = CreateBatch(x => keys.Select(y => x.StringGetAsync(y)).ToArray());
            await task.Task;
            return CreateMap(keys, task.Results, RedisStringInputCase.Default);
        }

        public IDictionary<string, bool> SetString(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            return SetStringAsync(datas, cacheTime, cacheSetIf).GetAwaiter().GetResult();
        }

        public async Task<IDictionary<string, bool>> SetStringAsync(KeyValuePair<string, string>[] datas, TimeSpan? cacheTime, CacheSetIf cacheSetIf = CacheSetIf.Always)
        {
            var task = CreateBatch(x =>
                datas.Select(y =>
                    x.StringSetAsync(y.Key, y.Value, cacheTime, (When)cacheSetIf)).ToArray());
            await task.Task;
            var map = new Dictionary<string, bool>(datas.Length);
            for (int i = 0; i < task.Results.Length; i++)
            {
                map[datas[i].Key] = task.Results[i].Result;
            }
            return map;
        }

        public IDictionary<string, bool> Delete(IReadOnlyList<string> keys)
        {
            return DeleteAsync(keys).GetAwaiter().GetResult();
        }

        public async Task<IDictionary<string, bool>> DeleteAsync(IReadOnlyList<string> keys)
        {
            var task = CreateBatch(x => keys.Select(y => x.KeyDeleteAsync(y)).ToArray());
            await task.Task;
            return CreateMap(keys, task.Results);
        }

        public IDictionary<string, bool> Expire(IReadOnlyList<string> keys, TimeSpan? cacheTime)
        {
            return ExpireAsync(keys, cacheTime).GetAwaiter().GetResult();
        }

        public async Task<IDictionary<string, bool>> ExpireAsync(IReadOnlyList<string> keys, TimeSpan? cacheTime)
        {
            var task = CreateBatch(x => keys.Select(y => x.KeyExpireAsync(y, cacheTime)).ToArray());
            await task.Task;
            return CreateMap(keys, task.Results);
        }
    }
}
