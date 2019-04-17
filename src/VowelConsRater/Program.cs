using System;
using StackExchange.Redis;
using System.Text.RegularExpressions;

namespace VowelConsRater
{
    class Program
    {
        public const String REDIS_HOST = "127.0.0.1:6379";
        public const String QUEUE_NAME_RATER = "vowel-cons-rater-jobs"; 
        public const String QUEUE_CHANNEL_RATER = "RateVowelConsJob"; 
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
        static void Main(string[] args)
        {
            var subsc = redis.GetSubscriber();
            subsc.Subscribe(QUEUE_CHANNEL_RATER, delegate 
            {
                IDatabase db = redis.GetDatabase();
                String message = db.ListRightPop(QUEUE_NAME_RATER);

                while(message != null)
                {
                    String[] results = message.Split(' ');
                    String id = results[0];
                    int countOfVowel = Convert.ToInt32(results[1]);
                    int countOfConsonant = Convert.ToInt32(results[2]);
                    Double relation = 0;

                    if (countOfConsonant != 0)
                    {
                        relation = countOfVowel/countOfConsonant;
                    }
                    db.StringSet("calculate: " + id, relation);
                    Console.WriteLine(message);
                    message = null;
                    message = db.ListRightPop(QUEUE_NAME_RATER);
                }
            });
            Console.ReadKey();
        }
    }
}