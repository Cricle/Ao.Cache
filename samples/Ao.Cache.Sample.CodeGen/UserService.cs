using Ao.Cache.Annotations;
using LiteDB;

namespace Ao.Cache.Sample.CodeGen
{
    [CacheProxy(ProxyAll = false)]
    public class UserService
    {
        public UserService(ICacheHelperCreator creator, ILiteDatabase database)
        {
            CacheCreator = creator;
            Database = database;
            Users = database.GetCollection<User>(BsonAutoId.Int64);
            Users.EnsureIndex(x => x.Name);
        }

        public ICacheHelperCreator CacheCreator { get; }

        public ILiteDatabase Database { get; }

        public ILiteCollection<User> Users { get; }

        [CacheProxyMethod(CacheTime = "00:01:00")]
        public virtual User[] AllUser()
        {
            Console.WriteLine("Run in raw method");
            return Users.FindAll().ToArray();
        }

        [CacheProxyMethod(CacheTime = "00:01:00")]
        public virtual User GetUser(string name)
        {
            Console.WriteLine("Run in raw method");
            return Users.Find(x => x.Name == name).FirstOrDefault();
        }
        public virtual BsonValue Add(string name)
        {
            var res = Users.Insert(new User { Name = name, Number = Random.Shared.Next(0, 9999) });
            CacheCreator.Delete(() => AllUser());
            return res;
        }
        public virtual int AddRange(int count)
        {
            var res = Users.Insert(Enumerable.Range(0, count).Select(x => new User
            {
                Name = Random.Shared.Next(0, 99999).ToString(),
                Number = Random.Shared.Next(0, 9999)
            }));
            CacheCreator.Delete(() => AllUser());
            return res;
        }
        public virtual BsonValue Delete(string name)
        {
            var res = Users.DeleteMany(x => x.Name == name);

            CacheCreator.Delete(() => AllUser());
            CacheCreator.Delete(() => GetUser(name));
            return res;
        }
        public int Clear()
        {
            var res = Users.DeleteAll();
            CacheCreator.Delete(() => AllUser());
            return res;
        }
    }
}