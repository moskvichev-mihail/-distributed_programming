using System;
using StackExchange.Redis;
using System.Configuration;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        static void Main(string[] args)
        {
            var subsc = redis.GetSubscriber();
            subsc.Subscribe("events", (channel, id) => {
                var db = redis.GetDatabase();
                var message = db.StringGet((string)id);
                Console.WriteLine("Id: " + (string)id);
                Console.WriteLine("Message: " + (string)message);
            });
            Console.ReadKey();
        }
    }
}