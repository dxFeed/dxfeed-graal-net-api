namespace DxLink.Net
{
    public class NameAndData
    {
        public string Name { get; set; } = "";
        public string Data { get; set; } = "";
    }

    public class MinimalCandleHolder
    {
        public MinimalCandleInternalHolder data { get; set; } = new MinimalCandleInternalHolder();
    }
    public class MinimalCandleInternalHolder
    {
        public List<MinimalStringCandle> items { get; set; } = new();
    }

    public class MinimalStringCandle
    {
        public string high { get; set; }
        public string low { get; set; }
        public string open { get; set; }
        public string close { get; set; }
        public string time { get; set; }
    }

    public class MinimalCandle
    {
        public double high { get; set; }
        public double low { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public long time { get; set; }
        public long esttime
        {
            get
            {
                TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);

                double offsetMilliseconds = offset.TotalMilliseconds;

                return time + (long)offsetMilliseconds;

                //DateTimeOffset utcDateTime = DateTimeOffset.FromUnixTimeMilliseconds(time);

                //DateTimeOffset localDateTime = utcDateTime.ToLocalTime();

                //return localDateTime.ToUnixTimeMilliseconds();
      
            }
        }


        // Get the local time given a long value representing the number of milliseconds since the epoch.
        public DateTime GetLocalTime()
        {
            DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeMilliseconds(time);
            return dateTime.LocalDateTime;
        }
    }

    public class Notification
    {
        public string Message { get; set; }
    }
    public class NameHolder
    {
        public string Name { get; set; }
    }
}
