using System;
using Microsoft.AspNetCore.Mvc;
using Clockwork.API.Models;

namespace Clockwork.API.Controllers
{    
    public class CurrentTimeController : Controller
    {
        public class TimeModel
        {
            public int Hours { get; set; }
            public int Minutes { get; set; }
            public int Seconds { get; set; }
            public bool IsDaylightSavings { get; set; }
        }
        // Post api/currenttime
        [HttpPost]
        [Route("api/[controller]")]
        public IActionResult Post([FromBody] TimeModel timeOffset)//int timeID  TimeModel timeOffset
        {
            if (timeOffset.Hours < 0)
            {
                timeOffset.Minutes = -timeOffset.Minutes;
                timeOffset.Seconds = -timeOffset.Seconds;
            }


            //Stores requested timeszone's metadata
            //var timeZone = TimeZoneInfo.GetSystemTimeZones()[timeID];
            var utcTime = DateTime.UtcNow;
            //Get server time in the requested time zone
            //var serverTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(timeZone.Id)); 
            var serverTime = DateTime.Now.ToUniversalTime();
            var serverTimeZoneAdjusted = serverTime.AddHours(timeOffset.Hours).AddMinutes(timeOffset.Minutes);
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
            //var timeZoneStamp = serverTime.GetUtcOffset(serverTime).ToString();
            var timeZoneStamp = serverTimeZoneAdjusted.Subtract(serverTime).ToString();
            var test = serverTime.IsDaylightSavingTime();
            //var timeZoneStamp = timeZone.GetUtcOffset(serverTime).ToString();
           



            var returnVal = new CurrentTimeQuery
            {
                UTCTime = utcTime,
                ClientIp = ip,
                Time = serverTime,
                TimeZoneStamp = timeZoneStamp
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
            return Ok(returnVal);
        }
        // GET api/currenttime/zoneselector
        [HttpGet]
        [Route("api/[controller]/zoneselector")]
        public IActionResult ZoneSelector()
        {
            return Ok(TimeZoneInfo.GetSystemTimeZones());
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
