using System;
using StackExchange.Redis;

namespace TextListener
{
    class Program
    {
        static void Main(string[] args)
        {
            Redis redis = new Redis();
            ISubscriber sub = redis.Sub();

            sub.Subscribe("events", (channel, message) => {
                string id = message;
                if (id.Contains("TextRankCalc_")) {
                    string valueFromMainDB = redis.GetStrFromDB(0, id);
                    string text = redis.GetStrFromDB(GetDatabaseId(valueFromMainDB), id);
                    ShowProcess(id, valueFromMainDB, text);
                }
            });
            
            Console.ReadLine();
        }
        
        private static int GetDatabaseId(string key)
        {
            return Convert.ToInt32(key);
        }

        public static void ShowProcess(string data, string region, string text)
        {   
            Console.WriteLine("ID: " + data);
            Console.WriteLine("Text: " + text);
            Console.WriteLine("REGION: " + region);
            Console.WriteLine("----------------------------------------");
        }
    }
}