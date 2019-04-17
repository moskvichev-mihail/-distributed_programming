using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.Threading;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public const String REDIS_HOST = "127.0.0.1:6379";
        private static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(REDIS_HOST);
        static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();

        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get(string id)
        {
            string value = null;
            var db = redis.GetDatabase();
            
            while(true)
            {
                if (db.KeyExists("calculate: " + id))
                {
                    value = db.StringGet("calculate: " + id);
                    break;
                }
                else
                {
                    Thread.Sleep(500);
                }
            }

            return value;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]string value)
        {
            var id = Guid.NewGuid().ToString();
            _data[id] = value;
            IDatabase db = redis.GetDatabase();
            db.StringSet(id, value);
            var subsc = redis.GetSubscriber();
            subsc.Publish("events", id);
            return id;
        }
    }
}