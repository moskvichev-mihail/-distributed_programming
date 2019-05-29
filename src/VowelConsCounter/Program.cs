using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace VowelConsCounter
{
    class Program
    {
        const string COUNTER_QUEUE_NAME  = "vowel-cons-counter-jobs";
        const string COUNTER_HINTS_CHANNEL  = "counter-hints";
        const string RATER_QUEUE_NAME  = "vowel-cons-rater-jobs";
        const string RATER_HINTS_CHANNEL  = "rate-hints";
       
        private static HashSet<char> VOWELS = new HashSet<char>{'a', 'e', 'i', 'o', 'u', 'y'};
		private static HashSet<char> CONSONANTS = new HashSet<char>{'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'};

        static void Main(string[] args)
        {
            Redis redis = new Redis();
            ISubscriber sub = redis.Sub();
            IDatabase getDB = redis.GetDB(0);

            sub.Subscribe(COUNTER_HINTS_CHANNEL, delegate
            {
                string msg = getDB.ListRightPop(COUNTER_QUEUE_NAME);
                while (msg != null)
                {
                    string id = ParseData(msg, 0);
                    string valueFromMainDB = redis.GetStrFromDB(0, id);
                    string valueFromRegionDB = redis.GetStrFromDB( GetDatabaseId(valueFromMainDB), id);
                    
                    int vowels = 0;
                    int consonants = 0;

                    foreach (char ch in valueFromRegionDB)
                    {
                        char chToLower = Char.ToLower(ch);
                        if (VOWELS.Contains(chToLower))
                        {
                            ++vowels;
                        }
                        else if (CONSONANTS.Contains(chToLower))
                        {
                            ++consonants;
                        }
                    }
                    SendMessage($"{id}:{vowels}:{consonants}", getDB, sub);
                    msg = getDB.ListRightPop( COUNTER_QUEUE_NAME );
                    ShowProcess(id, valueFromMainDB);
                }
            });
            
            Console.Title = "VowelConsCounter";
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
        
        private static int GetDatabaseId(string key)
        {
            return Convert.ToInt32(key);
        }

        private static void SendMessage(string message, IDatabase db, ISubscriber sub )
        {
            // put message to queue
            db.ListLeftPush( RATER_QUEUE_NAME, message, flags: CommandFlags.FireAndForget );
            // and notify consumers
            sub.Publish( RATER_HINTS_CHANNEL, "" );
        }

        private static string ParseData( string msg, int pos )
        {
            return msg.Split( ':' )[pos];
        }
        
        public static void ShowProcess(string data, string region)
        {   
            Console.WriteLine("ID: " + data);
            Console.WriteLine("REGION: " + region);
            Console.WriteLine("----------------------------------------");
        }
    }
}