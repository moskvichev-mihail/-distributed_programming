using System;
using StackExchange.Redis;

namespace VowelConsRater
{
    class Redis
    {
        IDatabase db;
        ConnectionMultiplexer redis;
        ISubscriber sub;

        public Redis()
        {
            redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        }

        public void Add(int databaseId, string key, string value)
        {
            db = redis.GetDatabase(databaseId);
            db.StringSet(key, value);
        }

        public void Publish(string key)
        {
            sub = redis.GetSubscriber();
            sub.Publish("events", key);    
        }

        public ISubscriber Sub()
        {
            return redis.GetSubscriber();   
        }

        public IDatabase GetDB(int databaseId)
        {
            return redis.GetDatabase(databaseId);   
        }

        public string GetStrFromDB(int databaseId, string key)
        {
            db = redis.GetDatabase(databaseId);
            return db.StringGet(key);
        }

        public static int GetDatabaseId(string regionCode)
        {
            switch (regionCode) 
            {
                case "rus":
                    return 1;
                case "eu":
                    return 2;
                case "usa":
                    return 3;
                default:
                    throw new Exception("Wrong region");
            }
        }
    }
}