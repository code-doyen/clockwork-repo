using System;
using Microsoft.AspNetCore.Mvc;
using Clockwork.API.Models;

namespace Clockwork.API.Controllers
{    
    public class CurrentTimeController : Controller
    {
        // GET api/currenttime
        [HttpGet]
        [Route("api/[controller]")]
        public IActionResult Get(int timeID)
        {
            Console.WriteLine(timeID);
            var utcTime = DateTime.UtcNow;
            var serverTime = DateTime.Now;         
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();

            var returnVal = new CurrentTimeQuery
            {
                UTCTime = utcTime,
                ClientIp = ip,
                Time = serverTime
            };

            using (var db = new ClockworkContext())
            {
                db.CurrentTimeQueries.Add(returnVal);
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine();
                foreach (var CurrentTimeQuery in db.CurrentTimeQueries)
                {
                    Console.WriteLine(" - {0}", CurrentTimeQuery.UTCTime);
                }
                
            }
            Console.WriteLine(serverTime.AddHours(3).ToLocalTime());
            return Ok(returnVal);
        }
        // GET api/selectall
        [HttpGet]
        [Route("api/[controller]/selectall")]
        public IActionResult SelectAll()
        {
             return Ok(new ClockworkContext().CurrentTimeQueries);
        }
    }
}
