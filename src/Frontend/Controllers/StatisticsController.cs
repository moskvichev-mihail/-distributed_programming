using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;

namespace Frontend.Controllers
{
    public class StatisticsController : Controller
    {
        static readonly string url = "http://localhost:5000/api/statistics/{0}";

        [HttpGet]
        public async Task<IActionResult> TextStatistics()
        {
            string textStatistics = null; 
            HttpClient client = new HttpClient();
            string tail = "TextRankCalculated";
            HttpResponseMessage request = await client.GetAsync(String.Format(url, tail));
            textStatistics = await request.Content.ReadAsStringAsync();

            string textNum = ParseData(textStatistics, 0);
            string highRankPart = ParseData(textStatistics, 1);
            string avgRank = ParseData(textStatistics, 2);

            ViewData["TextNum"] = textNum;
            ViewData["HighRankPart"] = highRankPart;
            ViewData["AvgRank"] = avgRank;
        
            return View();
        }

        private static string ParseData( string msg, int pos )
        {
            return msg.Split( ':' )[pos];
        }
    }
}