using System;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        public const String REDIS_HOST = "127.0.0.1:6379";
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
        static void Main(string[] args)
        {
            var subsc = redis.GetSubscriber();
            subsc.Subscribe("events", (channel, id) => {
                var db = redis.GetDatabase();
                var message = db.StringGet((string)id);
                Console.WriteLine("id: " + (string)id);
                Console.WriteLine("message: " + (string)message);
            });
            Console.ReadKey();
        }
    }
}