using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.Threading;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        Redis redis = new Redis();
        static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();
        
        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get([FromRoute] string id)
        {
            string valueFromMainDB = null;
            string valueFromRegionDB = null;
            string text = "TextRankCalc_" + id;
            
            for (int i = 0; i < 5; i++)
            {
                valueFromMainDB = redis.GetStrFromDB(0, text);
                valueFromRegionDB = redis.GetStrFromDB( GetDatabaseId(valueFromMainDB), text);

                if (valueFromRegionDB == null)
				{
					Thread.Sleep(200);
				}
            }
            
            string data = valueFromRegionDB + ":" + valueFromMainDB;
            return data;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody]string value)
        {
            var id = Guid.NewGuid().ToString();
            
            string data = ParseData(value, 0);
            string regionCode = ParseData(value, 1);
            int regionDatabaseId = Redis.GetDatabaseId(regionCode);
            string contextId = "TextRankCalc_" + id;
           
            redis.Add(0, contextId, regionDatabaseId.ToString());
            redis.Add(regionDatabaseId, contextId, data);
            ShowProcess(contextId, regionCode + "(" + regionDatabaseId + ")");

            redis.Publish(contextId);
            return id;
        }

        private int GetDatabaseId(string key)
        {
            return Convert.ToInt32(key);
        }

        private static string ParseData( string msg, int pos )
        {
            return msg.Split( ':' )[pos];
        }

        public void ShowProcess(string data, string region)
        {   
            Console.WriteLine("ID: " + data);
            Console.WriteLine("REGION: " + region);
            Console.WriteLine("----------------------------------------");
        }
    }
}