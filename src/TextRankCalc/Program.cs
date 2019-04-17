using System;
using StackExchange.Redis;
using System.Text.RegularExpressions;

namespace TextRankCalc
{
    class Program
    {
        public const String REDIS_HOST = "127.0.0.1:6379";
        public const String QUEUE_NAME_COUNTER = "vowel-cons-counter-jobs";
        public const String QUEUE_CHANNEL_COUNTER = "CalculateVowelConsJob";
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
        static void Main(string[] args)
        {
            var subsc = redis.GetSubscriber();
            subsc.Subscribe("events", (channel, id) => {
                var db = redis.GetDatabase();
                String message = db.StringGet((string)id);
                Console.WriteLine("Text: " + (string)message);
                Console.WriteLine("Id: "+ id);
                message += " "  + id;
                db.ListLeftPush( QUEUE_NAME_COUNTER, message, flags: CommandFlags.FireAndForget );
                db.Multiplexer.GetSubscriber().Publish( QUEUE_CHANNEL_COUNTER, "" );
            });
            Console.ReadKey();
        }
    }
}