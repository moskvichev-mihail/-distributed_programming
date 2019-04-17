using System;
using StackExchange.Redis;
using System.Text.RegularExpressions;

namespace VowelConsCounter
{
    class Program
    {
        public const String REDIS_HOST = "127.0.0.1:6379";
        public const String QUEUE_NAME_COUNTER = "vowel-cons-counter-jobs";
        public const String QUEUE_CHANNEL_COUNTER = "CalculateVowelConsJob";
        public const String QUEUE_NAME_RATER = "vowel-cons-rater-jobs"; 
        public const String QUEUE_CHANNEL_RATER = "RateVowelConsJob"; 
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
        static void Main(string[] args)
        {
            var subsc = redis.GetSubscriber();
            subsc.Subscribe(QUEUE_CHANNEL_COUNTER, delegate
            {
                IDatabase db = redis.GetDatabase();
                String message = db.ListRightPop(QUEUE_NAME_COUNTER);
                
                while(message != null)
                {
                    String[] resultOfSplit = message.Split(' ');
                    String text = resultOfSplit[0];
                    String id = resultOfSplit[1];
                    int countOfVowel = Regex.Matches(text, @"[aiueoy]", RegexOptions.IgnoreCase).Count;
                    int countOfConsonant = text.Length - countOfVowel;
                    
                    message = $"{id} {countOfVowel} {countOfConsonant}";
                    db.ListLeftPush(QUEUE_NAME_RATER, message, flags: CommandFlags.FireAndForget);
                    db.Multiplexer.GetSubscriber().Publish(QUEUE_CHANNEL_RATER, "");
                    Console.WriteLine(message);
                    message = null;
                    message = db.ListRightPop(QUEUE_NAME_COUNTER);
                }
            });
            Console.ReadKey();
        }
    }
}