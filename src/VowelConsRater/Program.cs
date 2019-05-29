using System;
using StackExchange.Redis;

namespace VowelConsRater
{
    class Program
    { 
        const string RATER_QUEUE_NAME  = "vowel-cons-rater-jobs";
        const string RATER_HINTS_CHANNEL  = "rate-hints";
        
        static void Main(string[] args)
        {
            Redis redis = new Redis();
            ISubscriber sub = redis.Sub();
            IDatabase getDB = redis.GetDB(0);

            sub.Subscribe(RATER_HINTS_CHANNEL, delegate
            {
                string msg = getDB.ListRightPop(RATER_QUEUE_NAME);
                while (msg != null)
                {                   
                    string id = ParseData(msg, 0);
                    string vowels = ParseData(msg, 1);
                    string consonants = ParseData(msg, 2);    
                    string ratio = vowels + "/" + consonants;
                    string valueFromMainDB = redis.GetStrFromDB(0, id);
                    
                    DoJob( "RATIO: ", ratio ); 
                    ShowProcess(id, valueFromMainDB);
                    redis.Add(GetDatabaseId(valueFromMainDB), id, ratio);
                    msg = getDB.ListRightPop(RATER_QUEUE_NAME);
                }
            });
            
            Console.Title = "VowelConsRater";
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
        private static int GetDatabaseId(string key)
        {
            return Convert.ToInt32(key);
        }

        private static string ParseData( string msg, int pos )
        {
            return msg.Split( ':' )[pos];
        }
        
        private static void DoJob( string message, string jobData )
        {
            Console.WriteLine( $"{message}{jobData}" );
        }

        public static void ShowProcess(string data, string region)
        {   
            Console.WriteLine("ID: " + data);
            Console.WriteLine("REGION: " + region);
            Console.WriteLine("----------------------------------------");
        }
    }
}