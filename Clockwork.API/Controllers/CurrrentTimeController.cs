using System;
using Microsoft.AspNetCore.Mvc;
using Clockwork.API.Models;

namespace Clockwork.API.Controllers
{    
    public class CurrentTimeController : Controller
    {
        
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
            if (timeOffset.IsDaylightSavings)
            {
                timeOffset.Hours += 1;
            }

            var utcTime = DateTime.UtcNow;
            //Get server time in the requested time zone
            var serverTime = utcTime.AddHours(timeOffset.Hours).AddMinutes(timeOffset.Minutes).AddSeconds(timeOffset.Seconds);
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
            var timeZoneStamp = serverTime.Subtract(utcTime).ToString();

            //Stores requested timeszone's metadata
            //var timeZone = TimeZoneInfo.GetSystemTimeZones()[timeID];
            //var utcTime = DateTime.UtcNow;
            //var serverTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(timeZone.Id)); 
            //var ip = this.HttpContext.Connection.RemoteIpAddress.ToString()
            //var timeZoneStamp = serverTime.GetUtcOffset(serverTime).ToString();
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
