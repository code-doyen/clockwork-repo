using System;
using Microsoft.EntityFrameworkCore;

namespace Clockwork.API.Models
{
    public class ClockworkContext : DbContext
    {
        public DbSet<CurrentTimeQuery> CurrentTimeQueries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=clockwork.db");
        }
    }

    public class CurrentTimeQuery
    {
        public int CurrentTimeQueryId { get; set; }
        public DateTime Time { get; set; }
        public string ClientIp { get; set; }
        public DateTime UTCTime { get; set; }
        public string TimeZoneStamp { get; set; }
    }

    public class TimeModel
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public bool IsDaylightSavings { get; set; }
    }
}
