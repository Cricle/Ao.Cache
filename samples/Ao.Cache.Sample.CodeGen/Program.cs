using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Ao.Cache.Sample.CodeGen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var db = new LiteDatabase("filename=a.ldb");
            var provider = new ServiceCollection()
                .AddInMemoryFinder()
                .AddSingleton<ILiteDatabase>(db)
                .AddSingleton<UserService, Gen.UserServiceProxy>()
                .BuildServiceProvider();
            var ser = provider.GetRequiredService<UserService>();
            while (true)
            {
                try
                {
                    Console.Write("> ");
                    var c = Console.ReadLine();
                    var command = c.Split(' ');
                    var key = command[0][0];
                    Console.Clear();
                    Console.CursorLeft = 0;
                    var sw = Stopwatch.GetTimestamp();
                    if (key == 'a')
                    {
                        var users = ser.AllUser();
                        Console.WriteLine("Total:" + users.Length);
                        foreach (var item in users)
                        {
                            Console.WriteLine(item);
                        }
                    }
                    else if (key == 'f')
                    {
                        Console.WriteLine(ser.GetUser(command[1]));
                    }
                    else if (key == 'i')
                    {
                        Console.WriteLine(ser.Add(command[1]).ToString());
                    }
                    else if (key == 'r')
                    {
                        var count = int.Parse(command[1]);
                        Console.WriteLine(ser.AddRange(count));
                    }
                    else if (key == 'd')
                    {
                        Console.WriteLine(ser.Delete(command[1]).ToString());
                    }
                    else if (key == 'p')
                    {
                        db.Checkpoint();
                        Console.WriteLine("ok");
                    }
                    else if (key == 'c')
                    {
                        Console.WriteLine(ser.Clear());
                    }
                    else if (key == 'q')
                    {
                        break;
                    }
                    Console.WriteLine(Stopwatch.GetElapsedTime(sw));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}