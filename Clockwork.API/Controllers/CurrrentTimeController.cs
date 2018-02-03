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
        public IActionResult Post([FromBody] int timeID)
        {
            //Stores requested timeszone's metadata
            var timeZone = GetTimeZone(timeID);
            var utcTime = DateTime.UtcNow;
            //Get server time in the requested time zone
            var serverTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(timeZone.Id));         
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
            var timeZoneStamp = timeZone.DisplayName;

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
            var tableAlert = "";
            tableAlert += "<tr><td scope='row'>" + returnVal.CurrentTimeQueryId + "</td>";
            tableAlert += "<td scope='row'>" + returnVal.Time + "</td>";
            tableAlert += "<td scope='row'>" + returnVal.ClientIp + "</td>";
            tableAlert += "<td scope='row'>" + returnVal.UTCTime + "</td>";
            tableAlert += "<td scope='row'>" + returnVal.TimeZoneStamp + "</td></tr>";
            return Ok(tableAlert);
        }
        // GET api/currenttime/zoneselector
        [HttpGet]
        [Route("api/[controller]/zoneselector")]
        public IActionResult ZoneSelector()
        {
            var zones = "";
            foreach (var info in TimeZoneInfo.GetSystemTimeZones())
            {
                if(TimeZoneInfo.Local.DisplayName.Equals(info.DisplayName)){
                    zones += "<option selected>"+ info.DisplayName + "</option>";
                }
                else
                {
                    zones += "<option>" + info.DisplayName + "</option>";
                }
            }
            return Ok(zones);
        }
        // GET api/selectall
        [HttpGet]
        [Route("api/[controller]/selectall")]
        public IActionResult SelectAll()
        {
            var table = "";
            using (var db = new ClockworkContext())
            {
                foreach (var CurrentTimeQuery in db.CurrentTimeQueries)
                {
                    table += "<tr><td scope='row'>" + CurrentTimeQuery.CurrentTimeQueryId + "</td>";
                    table += "<td scope='row'>" + CurrentTimeQuery.Time + "</td>";
                    table += "<td scope='row'>" + CurrentTimeQuery.ClientIp + "</td>";
                    table += "<td scope='row'>" + CurrentTimeQuery.UTCTime + "</td>";
                    table += "<td scope='row'>" + CurrentTimeQuery.TimeZoneStamp + "</td></tr>";
                }

            }
            return Ok(table);
            //return Ok(new ClockworkContext().CurrentTimeQueries);
        }
        // Not an action method.
        [NonAction]
        private TimeZoneInfo GetTimeZone(int id) {
            return TimeZoneInfo.GetSystemTimeZones()[id];
        }
    }
}
